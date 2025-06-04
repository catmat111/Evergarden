using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoDW.Data;
using ProjetoDW.Models;

namespace ProjetoDW.Controllers
{
    public class UtilizadoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UtilizadoresController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UtilizadoresR
        [Authorize(Roles = "Remetente")]
        public async Task<IActionResult> Index()
        {
            var identityUser = await _userManager.GetUserAsync(User);

            var remetente = await _context.Utilizadores
                .Include(u => u.UtilizadoresDestinatarios)
                .FirstOrDefaultAsync(u => u.IdentityUserID == identityUser.Id);

            if (remetente == null)
            {
                return NotFound("Remetente não encontrado.");
            }

            var meusDestinatarios = remetente.UtilizadoresDestinatarios;

            return View(meusDestinatarios);
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

            _context.Utilizadores.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }







        // GET: UtilizadoresR/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizadoresR = await _context.Utilizadores.FindAsync(id);
            if (utilizadoresR == null)
            {
                return NotFound();
            }
            return View(utilizadoresR);
        }

        // POST: UtilizadoresR/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Password,Imagem,Telemovel,Email,Idade,DataNascimento")] Utilizadores utilizadores)
        {
            if (id != utilizadores.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utilizadores);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadoresRExists(utilizadores.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return View("DestinatarioCriado","UtilizadoresR");
            }
            return View(utilizadores);
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilizadoresR = await _context.Utilizadores.FindAsync(id);
            if (utilizadoresR != null)
            {
                _context.Utilizadores.Remove(utilizadoresR);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilizadoresRExists(int id)
        {
            return _context.Utilizadores.Any(e => e.Id == id);
        }
    }
}
