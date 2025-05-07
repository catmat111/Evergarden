using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
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

            if (Input.TipoUtilizador == 0)
            {
                ModelState.AddModelError("TipoUtilizador", "Tem de escolher um tipo de utilizador");
            }
            
            if (ModelState.IsValid)
            {
                // Cria o utilizador IdentityUser (se necessário para autenticação)
                var user = new IdentityUser { UserName = Input.Utilizadores.Email, Email = Input.Utilizadores.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (Input.TipoUtilizador == 1)
                    {
                        IdentityUserRole<string> identityUserRole = new IdentityUserRole<string> { UserId = user.Id, RoleId = "REMET" };
                        _context.UserRoles.Add(identityUserRole);
                    }
                    else if (Input.TipoUtilizador == 2)
                    {
                        IdentityUserRole<string> identityUserRole = new IdentityUserRole<string> { UserId = user.Id, RoleId = "DEST" };
                        _context.UserRoles.Add(identityUserRole);
                    }
                    
                    
                    // Cria o UtilizadorR e regista na base de dados
                    var utilizadorR = new Utilizadores
                    {
                        Nome = Input.Utilizadores.Nome,
                        Email = Input.Utilizadores.Email,
                        Telemovel = Input.Utilizadores.Telemovel,
                        NIF = Input.Utilizadores.NIF,
                        ImagemPath = "",
                        DataNascimento = Input.Utilizadores.DataNascimento
                    };

                    _context.Utilizadores.Add(utilizadorR);
                    await _context.SaveChangesAsync();

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Utilizadores.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
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