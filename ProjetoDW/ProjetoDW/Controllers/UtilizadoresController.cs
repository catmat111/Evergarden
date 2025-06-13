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
    public class UtilizadoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public UtilizadoresController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: UtilizadoresR
        [Authorize(Roles = "Remetente")]
        public async Task<IActionResult> Index(string searchString)
        {
            var user = await _userManager.GetUserAsync(User);

            
            // Obtem o ID da role "Destinatario"
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Destinatario");
            if (role == null)
            {
                return Problem("A role 'Destinatario' não foi encontrada.");
            }

            // Obtem os UserIds de todos os utilizadores com essa role
            var userIdsComRoleDestinatario = await _context.UserRoles
                .Where(ur => ur.RoleId == role.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            // Obtem os utilizadores da tabela Utilizadores cujos IdentityUserId está na lista
            var utilizadoresQuery = _context.Utilizadores
                .Include(u => u.Remetente)
                .Where(u => userIdsComRoleDestinatario.Contains(u.IdentityUserID))
                .Where(u => u.Remetente.IdentityUserID == user.Id);

            // Se houver filtro de pesquisa
            if (!string.IsNullOrEmpty(searchString))
            {
                utilizadoresQuery = utilizadoresQuery
                    .Where(u => u.Nome.Contains(searchString));
            }

            var listaFinal = await utilizadoresQuery.ToListAsync();
            return View(listaFinal);
        }





        // GET: UtilizadoresR/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizadoresR = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizadoresR == null)
            {
                return NotFound();
            }

            return View(utilizadoresR);
        }

        // GET: UtilizadoresR/Create
        // GET: UtilizadoresR/Create
// GET: UtilizadoresR/Create
        [Authorize(Roles = "Remetente")]
        public IActionResult Create()
        {
            return View();
        }
        


        // POST: UtilizadoresR/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
// POST: UtilizadoresR/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Remetente")]
        public async Task<IActionResult> Create(Utilizadores model, string password)
        {
            
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var remetente = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);

            if (remetente == null)
                return View("ErroRemetente");

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

            await _userManager.AddToRoleAsync(newUser, "DESTINATARIO");
            model.ImagemPath = "default.png";
            // === GUARDAR IMAGEM ===
            if (model.Imagem != null && model.Imagem.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Imagem.FileName);
                var filePath = Path.Combine("wwwroot/recursos/imagens_user", fileName);

                // Criar pasta se não existir
                var folder = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Imagem.CopyToAsync(stream);
                }

                model.ImagemPath = "imagens_user/" + fileName; // Caminho para guardar na BD
            }

            model.IdentityUserID = newUser.Id;
            model.RemetenteId = remetente.Id;
            model.Telemovel = "+351 " + model.Telemovel;


            _context.Utilizadores.Add(model);
            await _context.SaveChangesAsync();
            
            // Geração do token de confirmação de email
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


            return View("ContaCriada");
        }







        // GET: UtilizadoresR/Edit/5
        [Authorize(Roles = "Remetente")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var remetente = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);

            if (remetente == null)
                return Unauthorized();

            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u =>
                u.Id == id &&
                (u.Id == remetente.Id || u.RemetenteId == remetente.Id)); // Pode editar-se a si ou aos destinatários criados

            if (utilizador == null)
                return NotFound();

            return View(utilizador);
        }


        // POST: UtilizadoresR/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

    model.Email = utilizador.Email;
    ModelState.Remove(nameof(model.Email)); // limpa o erro causado pelo campo vazio

    if (ModelState.IsValid)
    {
        try
        {
            // Atualiza dados
            utilizador.Nome = model.Nome;
            utilizador.Telemovel = model.Telemovel;
            utilizador.DataNascimento = model.DataNascimento;

            // Atualizar imagem se fornecida
            if (model.Imagem != null && model.Imagem.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Imagem.FileName);
                var filePath = Path.Combine("wwwroot/recursos/imagens_user", fileName);

                // Criar pasta se não existir
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
            return RedirectToAction(nameof(Index));
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



        // GET: UtilizadoresR/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizadoresR = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizadoresR == null)
            {
                return NotFound();
            }

            return View(utilizadoresR);
        }
        
        
        public IActionResult ContaCriada()
        {
            return View();
        }

        // POST: UtilizadoresR/Delete/5
        [HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
        [Authorize(Roles = "Remetente")]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var utilizador = await _context.Utilizadores
        .Include(u => u.UtilizadoresDestinatarios)
        .Include(u => u.Remetente)
        .FirstOrDefaultAsync(u => u.Id == id);

    if (utilizador == null)
        return NotFound();

    // Obter o IdentityUser relacionado
    var identityUserr = await _userManager.FindByIdAsync(utilizador.IdentityUserID);

    // Verificar se é Remetente ou Destinatário
    if (utilizador.RemetenteId == null)
    {
        // ➤ É um REMETENTE

        // 1. Eliminar cartas criadas por este remetente
        var cartasRemetente = _context.Cartas
            .Where(c => c.UtilizadorRemetenteFk == utilizador.Id);
        _context.Cartas.RemoveRange(cartasRemetente);

        // 2. Eliminar destinatários criados por ele
        foreach (var destinatario in utilizador.UtilizadoresDestinatarios)
        {
            // ➤ Eliminar cartas recebidas pelo destinatário
            var cartasDestinatario = _context.Cartas
                .Where(c => c.UtilizadorDestinatarioFk == destinatario.Id);
            _context.Cartas.RemoveRange(cartasDestinatario);

            // ➤ Eliminar o utilizador destinatário
            var identityDestinatario = await _userManager.FindByIdAsync(destinatario.IdentityUserID);
            if (identityDestinatario != null)
                await _userManager.DeleteAsync(identityDestinatario);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var remetente = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);

            if (remetente == null || (utilizador.Id != remetente.Id && utilizador.RemetenteId != remetente.Id))
            {
                return Unauthorized();
            }

            // Eliminar o IdentityUser primeiro
            var identityUser = await _userManager.FindByIdAsync(utilizador.IdentityUserID);
            if (identityUser != null)
            {
                await _userManager.DeleteAsync(identityUser);
            }

            // O Cascade trata de cartas, destinatários, categorias
            _context.Utilizadores.Remove(utilizador);
            await _context.SaveChangesAsync();

            return View("ContaDeletada");
            _context.Utilizadores.Remove(destinatario);
        }

        // 3. Eliminar categorias criadas por este remetente
        var categoriasCriadas = _context.Categorias
            .Where(c => c.UtilizadorCriadorId == utilizador.IdentityUserID);
        _context.Categorias.RemoveRange(categoriasCriadas);
    }
    else
    {
        // ➤ É um DESTINATÁRIO

        // 1. Eliminar cartas recebidas
        var cartasDestinatario = _context.Cartas
            .Where(c => c.UtilizadorDestinatarioFk == utilizador.Id);
        _context.Cartas.RemoveRange(cartasDestinatario);
    }

    // 4. Eliminar utilizador principal (Remetente ou Destinatário)
    _context.Utilizadores.Remove(utilizador);

    // 5. Eliminar IdentityUser (conta de login)
    if (identityUserr != null)
        await _userManager.DeleteAsync(identityUserr);

    await _context.SaveChangesAsync();
    return View("DestinatarioDeletado");
}






        private bool UtilizadoresRExists(int id)
        {
            return _context.Utilizadores.Any(e => e.Id == id);
        }
    }
}
