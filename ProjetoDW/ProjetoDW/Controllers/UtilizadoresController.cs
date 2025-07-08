using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using ProjetoDW.Data;
using ProjetoDW.Models;

namespace ProjetoDW.Controllers
{
    /// <summary>
    /// Controlador para gerir todas as operações relacionadas com os utilizadores,
    /// incluindo registo de destinatários, gestão de perfis e eliminação de contas.
    /// </summary>
    public class UtilizadoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<IdentityUser> _signInManager;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="UtilizadoresController"/>.
        /// </summary>
        /// <param name="context">O contexto da base de dados.</param>
        /// <param name="userManager">Serviço de gestão de utilizadores do Identity.</param>
        /// <param name="emailSender">Serviço para envio de emails.</param>
        /// <param name="signInManager">Serviço para gestão de login/logout.</param>
        public UtilizadoresController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEmailSender emailSender, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
        }

        // GET: Utilizadores
        /// <summary>
        /// Lista os destinatários criados pelo remetente autenticado.
        /// Apenas acessível por utilizadores com o papel "Remetente".
        /// </summary>
        /// <param name="searchString">Termo para filtrar destinatários por nome.</param>
        /// <returns>Uma View com a lista de destinatários filtrada.</returns>
        [Authorize(Roles = "Remetente")]
        public async Task<IActionResult> Index(string searchString)
        {
            var user = await _userManager.GetUserAsync(User);

            // Encontra o ID do papel "Destinatario".
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Destinatario");
            if (role == null)
            {
                return Problem("A role 'Destinatario' não foi encontrada.");
            }

            // Obtém os IDs de todos os utilizadores que têm o papel "Destinatario".
            var userIdsComRoleDestinatario = await _context.UserRoles
                .Where(ur => ur.RoleId == role.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            // Constrói a query para obter os utilizadores (da nossa tabela Utilizadores) que são destinatários
            // e que foram criados pelo remetente atualmente autenticado.
            var utilizadoresQuery = _context.Utilizadores
                .Include(u => u.Remetente)
                .Where(u => userIdsComRoleDestinatario.Contains(u.IdentityUserID))
                .Where(u => u.Remetente.IdentityUserID == user.Id);

            // Aplica o filtro de pesquisa, se existir.
            if (!string.IsNullOrEmpty(searchString))
            {
                utilizadoresQuery = utilizadoresQuery
                    .Where(u => u.Nome.Contains(searchString));
            }

            var listaFinal = await utilizadoresQuery.ToListAsync();
            return View(listaFinal);
        }

        // GET: Utilizadores/Details/5
        /// <summary>
        /// Apresenta os detalhes de um utilizador específico.
        /// </summary>
        /// <param name="id">O ID do utilizador.</param>
        /// <returns>Uma View com os detalhes do utilizador.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizadores = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizadores == null)
            {
                return NotFound();
            }

            return View(utilizadores);
        }

        // GET: Utilizadores/Create
        /// <summary>
        /// Apresenta o formulário para um remetente criar um novo destinatário.
        /// </summary>
        /// <returns>A View de criação.</returns>
        [Authorize(Roles = "Remetente")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Utilizadores/Create
        /// <summary>
        /// Processa a criação de um novo utilizador destinatário.
        /// Cria a conta no ASP.NET Identity, associa-a ao remetente, guarda a imagem e envia um email de confirmação.
        /// </summary>
        /// <param name="model">Os dados do novo utilizador.</param>
        /// <param name="password">A password para a nova conta.</param>
        /// <returns>Uma View de sucesso ou a View de criação com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Remetente")]
        public async Task<IActionResult> Create(Utilizadores model, string password)
        {
            if (!ModelState.IsValid)
                return View(model);
            
            // Encontra o perfil do remetente autenticado na nossa tabela Utilizadores.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var remetente = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);

            if (remetente == null)
                return View("ErroRemetente");

            // Cria o novo utilizador no sistema de Identity do ASP.NET.
            var newUser = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(newUser, password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }
            
            // Atribui o papel "DESTINATARIO" ao novo utilizador.
            await _userManager.AddToRoleAsync(newUser, "DESTINATARIO");

            // Define um caminho de imagem por defeito.
            model.ImagemPath = "default.png";

            // Processa e guarda a imagem de perfil, se uma for enviada.
            if (model.Imagem != null && model.Imagem.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Imagem.FileName);
                var filePath = Path.Combine("wwwroot/recursos/imagens_user", fileName);

                var folder = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Imagem.CopyToAsync(stream);
                }
                model.ImagemPath = "imagens_user/" + fileName;
            }
            
            // Associa o ID do IdentityUser e o ID do remetente ao novo perfil de utilizador.
            model.IdentityUserID = newUser.Id;
            model.RemetenteId = remetente.Id;

            _context.Utilizadores.Add(model);
            await _context.SaveChangesAsync();

            // Envia o email de confirmação da conta.
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = newUser.Id, code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                newUser.Email,
                "Confirmação de conta",
                $"Por favor confirma a tua conta <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");

            return View("DestinatarioCriado");
        }

        // GET: Utilizadores/Edit/5
        /// <summary>
        /// Apresenta o formulário para editar os dados de um utilizador.
        /// Um remetente pode editar os seus próprios dados ou os dos destinatários que criou.
        /// </summary>
        /// <param name="id">O ID do utilizador a editar.</param>
        /// <returns>A View de edição ou uma página de erro.</returns>
        [Authorize(Roles = "Remetente")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var remetente = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);

            if (remetente == null)
                return Unauthorized();

            // Verifica se o utilizador a editar é o próprio remetente ou um dos seus destinatários.
            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u =>
                u.Id == id &&
                (u.Id == remetente.Id || u.RemetenteId == remetente.Id));

            if (utilizador == null)
                return NotFound();

            return View(utilizador);
        }

        // POST: Utilizadores/Edit/5
        /// <summary>
        /// Processa a atualização dos dados de um utilizador.
        /// </summary>
        /// <param name="id">O ID do utilizador a ser atualizado.</param>
        /// <param name="model">Os dados atualizados do utilizador.</param>
        /// <returns>Uma View de sucesso ou a View de edição com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Remetente")]
        public async Task<IActionResult> Edit(int id, Utilizadores model)
        {
            if (id != model.Id)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var remetente = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);

            if (remetente == null)
                return Unauthorized();

            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u =>
                u.Id == id &&
                (u.Id == remetente.Id || u.RemetenteId == remetente.Id));

            if (utilizador == null)
                return NotFound();
            
            // O email não pode ser alterado, por isso é preservado e removido da validação do modelo.
            model.Email = utilizador.Email;
            ModelState.Remove(nameof(model.Email));

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualiza os dados do utilizador existente.
                    utilizador.Nome = model.Nome;
                    utilizador.Telemovel = model.Telemovel;
                    utilizador.DataNascimento = model.DataNascimento;

                    // Atualiza a imagem se uma nova for fornecida.
                    if (model.Imagem != null && model.Imagem.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Imagem.FileName);
                        var filePath = Path.Combine("wwwroot/recursos/imagens_user", fileName);

                        var folder = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(folder))
                            Directory.CreateDirectory(folder);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.Imagem.CopyToAsync(stream);
                        }
                        utilizador.ImagemPath = "imagens_user/" + fileName;
                    }

                    await _context.SaveChangesAsync();
                    return View("ContaEditada", utilizador);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadoresRExists(model.Id))
                        return NotFound();
                    throw;
                }
            }
            return View(model);
        }
        
        /// <summary>
        /// Apresenta a página de perfil do utilizador autenticado.
        /// </summary>
        /// <returns>A View de perfil com os dados do utilizador.</returns>
        public async Task<IActionResult> Perfil()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.IdentityUserID == identityUser.Id);

            if (utilizador == null)
            {
                return NotFound();
            }

            return View(utilizador);
        }

        // GET: Utilizadores/Delete/5
        /// <summary>
        /// Apresenta a página de confirmação para eliminar uma conta.
        /// </summary>
        /// <param name="id">O ID do utilizador a eliminar.</param>
        /// <returns>A View de confirmação.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizadores = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizadores == null)
            {
                return NotFound();
            }

            return View(utilizadores);
        }

        /// <summary>
        /// Apresenta uma página de confirmação genérica.
        /// </summary>
        /// <returns>A View "ContaCriada".</returns>
        public IActionResult ContaCriada()
        {
            return View();
        }

        // POST: Utilizadores/Delete/5
        /// <summary>
        /// Confirma e executa a eliminação de uma conta de utilizador e todos os seus dados associados.
        /// </summary>
        /// <param name="id">O ID do utilizador a ser eliminado.</param>
        /// <returns>Redireciona para a página inicial após a eliminação.</returns>
        [HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Remetente")]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var utilizador = await _context.Utilizadores
        .Include(u => u.UtilizadoresDestinatarios) // Inclui os destinatários para eliminação em cascata
        .FirstOrDefaultAsync(u => u.Id == id);

    if (utilizador == null)
        return NotFound();

    var identityUser = await _userManager.FindByIdAsync(utilizador.IdentityUserID);

    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    // Obter o remetente autenticado
    var remetente = await _context.Utilizadores
        .FirstOrDefaultAsync(u => u.IdentityUserID == currentUserId);

    // Permitir eliminação apenas se for o próprio ou se for um remetente a eliminar um destinatário que criou
    if (utilizador.IdentityUserID != currentUserId &&
        utilizador.RemetenteId != remetente?.Id)
    {
        return Unauthorized();
    }

    // Lógica de eliminação
    if (utilizador.RemetenteId == null)
    {
        // REMETENTE: Elimina cartas, categorias e destinatários associados
        _context.Cartas.RemoveRange(_context.Cartas.Where(c => c.UtilizadorRemetenteFk == utilizador.Id));
        _context.Categorias.RemoveRange(_context.Categorias.Where(c => c.UtilizadorCriadorId == utilizador.IdentityUserID));

        foreach (var dest in utilizador.UtilizadoresDestinatarios)
        {
            _context.Cartas.RemoveRange(_context.Cartas.Where(c => c.UtilizadorDestinatarioFk == dest.Id));
            var destIdentity = await _userManager.FindByIdAsync(dest.IdentityUserID);
            if (destIdentity != null) await _userManager.DeleteAsync(destIdentity);
            _context.Utilizadores.Remove(dest);
        }
    }
    else
    {
        // DESTINATÁRIO: Elimina apenas as cartas onde é destinatário
        _context.Cartas.RemoveRange(_context.Cartas.Where(c => c.UtilizadorDestinatarioFk == utilizador.Id));
    }

    // Remove da tabela de Utilizadores
    _context.Utilizadores.Remove(utilizador);
    await _context.SaveChangesAsync();

    // Apenas termina sessão se for o próprio
    if (utilizador.IdentityUserID == currentUserId)
    {
        await _signInManager.SignOutAsync();
    }

    // Elimina o utilizador do Identity
    if (identityUser != null)
    {
        await _userManager.DeleteAsync(identityUser);
    }

    return View("DestinatarioDeletado");
}

        
        /// <summary>
        /// Verifica se um utilizador existe na base de dados.
        /// </summary>
        /// <param name="id">O ID do utilizador.</param>
        /// <returns>Verdadeiro se o utilizador existir, falso caso contrário.</returns>
        private bool UtilizadoresRExists(int id)
        {
            return _context.Utilizadores.Any(e => e.Id == id);
        }
    }
}
