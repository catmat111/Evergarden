using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using ProjetoDW.Models;
using ProjetoDW.Data;

namespace ProjetoDW.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            ILogger<RegisterModel> logger,
            ApplicationDbContext context,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
        }

        [BindProperty] public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            public Utilizadores Utilizadores { get; set; }

            public string Password { get; set; }

            [Display(Name = "Tipo de Utilizador")]
            [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
            public int TipoUtilizador { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");


            if (ModelState.IsValid)
            {
                // Cria o utilizador IdentityUser (se necessário para autenticação)
                var user = new IdentityUser { UserName = Input.Utilizadores.Email, Email = Input.Utilizadores.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

// Define a role dinamicamente (ajustável conforme a tua lógica)
                    await _userManager.AddToRoleAsync(user, "REMETENTE");


// Preparar imagem
                    string nomeImagem = "default.png";
                    IFormFile teste = Input.Utilizadores.Imagem;
                    if (Input.Utilizadores.Imagem != null)
                    {
                        var imagem = Input.Utilizadores.Imagem;
                        if (imagem.ContentType == "image/jpeg" || imagem.ContentType == "image/png")
                        {
                            var guid = Guid.NewGuid().ToString();
                            var extensao = Path.GetExtension(imagem.FileName).ToLowerInvariant();
                            nomeImagem = guid + extensao;

                            var pathPasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/recursos/imagens_user");
                            if (!Directory.Exists(pathPasta))
                                Directory.CreateDirectory(pathPasta);

                            var caminhoFinal = Path.Combine(pathPasta, nomeImagem);
                            using (var stream = new FileStream(caminhoFinal, FileMode.Create))
                            {
                                await imagem.CopyToAsync(stream);
                            }

                            nomeImagem = "imagens_user/" + nomeImagem;
                        }
                        else
                        {
                            ModelState.AddModelError("Imagem", "Apenas ficheiros .jpg e .png são permitidos.");
                            return Page();
                        }
                    }

// Criar o utilizador de negócio
                    var utilizadorR = new Utilizadores
                    {
                        Nome = Input.Utilizadores.Nome,
                        Email = Input.Utilizadores.Email,
                        Telemovel = Input.Utilizadores.Telemovel,
                        ImagemPath = nomeImagem,
                        DataNascimento = Input.Utilizadores.DataNascimento,
                        IdentityUserID = user.Id
                    };

// Se for destinatário, assume o remetente atualmente autenticado como criador
                    if (Input.TipoUtilizador == 2)
                    {
                        var remetenteIdentityUser = await _userManager.GetUserAsync(User);
                        var remetente =
                            _context.Utilizadores.FirstOrDefault(u => u.IdentityUserID == remetenteIdentityUser.Id);
                        if (remetente != null)
                        {
                            utilizadorR.RemetenteId = remetente.Id;
                        }
                    }

                    _context.Utilizadores.Add(utilizadorR);
                    

                    await _context.SaveChangesAsync();

                    return RedirectToAction("ContaCriada", "Utilizadores");


                }

                // Se falhar, adiciona erros ao ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Se algo falhar, retorna a mesma página com erros
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