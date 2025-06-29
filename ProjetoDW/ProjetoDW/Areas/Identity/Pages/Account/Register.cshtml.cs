using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using ProjetoDW.Models;
using ProjetoDW.Data;

namespace ProjetoDW.Areas.Identity.Pages.Account
{
    /// <summary>
    /// PageModel que gere a lógica para o registo de novos utilizadores na aplicação.
    /// </summary>
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            ILogger<RegisterModel> logger,
            ApplicationDbContext context,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Modelo que contém os dados submetidos pelo formulário de registo.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// URL para o qual o utilizador será redirecionado após o registo.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Classe interna que define a estrutura dos dados do formulário.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// Contém os dados do perfil do utilizador (Nome, Email, etc.).
            /// </summary>
            public Utilizadores Utilizadores { get; set; }

            /// <summary>
            /// A password escolhida pelo utilizador.
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// Define se o utilizador é um Remetente (1) ou Destinatário (2).
            /// </summary>
            [Display(Name = "Tipo de Utilizador")]
            [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
            public int TipoUtilizador { get; set; }
        }

        /// <summary>
        /// Método HTTP GET para inicializar a página de registo.
        /// </summary>
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        /// <summary>
        /// Método HTTP POST para processar a submissão do formulário de registo.
        /// </summary>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // 1. Cria o utilizador no sistema de Identity do ASP.NET Core
                var user = new IdentityUser { UserName = Input.Utilizadores.Email, Email = Input.Utilizadores.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // 2. Atribui o papel (role) de "REMETENTE" ao novo utilizador.
                    await _userManager.AddToRoleAsync(user, "REMETENTE");

                    // 3. Processa e guarda a imagem de perfil, se fornecida.
                    string nomeImagem = "default.png"; // Imagem padrão
                    if (Input.Utilizadores.Imagem != null)
                    {
                        var imagem = Input.Utilizadores.Imagem;
                        // Valida o tipo de ficheiro
                        if (imagem.ContentType == "image/jpeg" || imagem.ContentType == "image/png")
                        {
                            // Gera um nome único para o ficheiro para evitar conflitos.
                            var guid = Guid.NewGuid().ToString();
                            var extensao = Path.GetExtension(imagem.FileName).ToLowerInvariant();
                            nomeImagem = guid + extensao;

                            // Define o caminho completo para guardar a imagem.
                            var pathPasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/recursos/imagens_user");
                            if (!Directory.Exists(pathPasta))
                                Directory.CreateDirectory(pathPasta);

                            var caminhoFinal = Path.Combine(pathPasta, nomeImagem);
                            // Guarda o ficheiro no servidor.
                            using (var stream = new FileStream(caminhoFinal, FileMode.Create))
                            {
                                await imagem.CopyToAsync(stream);
                            }
                            // Guarda o caminho relativo para ser usado na BD.
                            nomeImagem = "imagens_user/" + nomeImagem;
                        }
                        else
                        {
                            ModelState.AddModelError("Imagem", "Apenas ficheiros .jpg e .png são permitidos.");
                            return Page();
                        }
                    }

                    // 4. Cria o registo correspondente na nossa tabela customizada 'Utilizadores'.
                    var utilizadorCustomizado = new Utilizadores
                    {
                        Nome = Input.Utilizadores.Nome,
                        Email = Input.Utilizadores.Email,
                        Telemovel = Input.Utilizadores.Telemovel,
                        ImagemPath = nomeImagem,
                        DataNascimento = Input.Utilizadores.DataNascimento,
                        IdentityUserID = user.Id // Liga o nosso perfil ao utilizador do Identity.
                    };

                    // Se o tipo for 2 (Destinatário), associa o remetente que o está a criar.
                    if (Input.TipoUtilizador == 2)
                    {
                        var remetenteIdentityUser = await _userManager.GetUserAsync(User);
                        var remetente = _context.Utilizadores.FirstOrDefault(u => u.IdentityUserID == remetenteIdentityUser.Id);
                        if (remetente != null)
                        {
                            utilizadorCustomizado.RemetenteId = remetente.Id;
                        }
                    }
                    
                    // Guarda o novo perfil na base de dados.
                    _context.Utilizadores.Add(utilizadorCustomizado);
                    await _context.SaveChangesAsync();
                    
                    // 5. Gera o token de confirmação e envia o email.
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    // Cria o link de confirmação que o utilizador irá clicar.
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code },
                        protocol: Request.Scheme);
                    
                    // Envia o email usando o serviço IEmailSender.
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Confirmação de conta",
                        $"Por favor confirma a tua conta <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");

                    // Redireciona para uma página de confirmação.
                    return RedirectToPage("RegisterConfirmation", new { email = user.Email });
                }

                // Se a criação do utilizador no Identity falhar, adiciona os erros ao ModelState.
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Se o ModelState for inválido, retorna a mesma página para exibir os erros.
            return Page();
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}