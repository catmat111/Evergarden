using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoDW.Data;
using ProjetoDW.Models;
using Microsoft.AspNetCore.Identity;

namespace ProjetoDW.Controllers
{
    public class CartasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Cartas
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var tarefas = await _context.Tarefa
                .Where(t => t.UtilizadorId == userId)
                .ToListAsync();

            ViewBag.Tarefas = tarefas;

            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);

            var cartas = await _context.Cartas
                .Include(c => c.UtilizadorRemetente)
                .Include(c => c.UtilizadorDestinatario)
                .Where(c => c.UtilizadorRemetenteFk == utilizador.Id || c.UtilizadorDestinatarioFk == utilizador.Id)
                .ToListAsync();


            return View(cartas);
        }



        // GET: Cartas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var carta = await _context.Cartas
                .Include(c => c.UtilizadorRemetente)
                .Include(c => c.UtilizadorDestinatario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (carta == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);

            if (carta.UtilizadorRemetenteFk != utilizador.Id && carta.UtilizadorDestinatarioFk != utilizador.Id)
                return Forbid();

            return View(carta);
        }


        // GET: Cartas/Create
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == userId);
            var remetente = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);
            var eremet = User.IsInRole("REMET");
            if (remetente == null || !User.IsInRole("REMET"))
            {
                return Forbid();
            }

            var destinatarios = await _context.Utilizadores
                .Where(u => u.RemetenteId == remetente.Id)
                .ToListAsync();

            ViewData["UtilizadorDestinatarioFk"] = new SelectList(destinatarios, "Id", "Nome");
            return View();
            
        }

        // POST: Cartas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titulo,Descricao,Topico,DataEnvio,UtilizadorDestinatarioFk")] Cartas cartas)
        {
            var userId = _userManager.GetUserId(User);
            var remetente = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);

            if (remetente == null || !User.IsInRole("REMETENTE"))
                return Forbid();

            if (ModelState.IsValid)
            {
                cartas.DataCriacao = DateTime.Now;
                cartas.UtilizadorRemetenteFk = remetente.Id;

                _context.Add(cartas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var destinatarios = await _context.Utilizadores
                .Where(u => u.RemetenteId == remetente.Id)
                .ToListAsync();

            ViewData["UtilizadorDestinatarioFk"] = new SelectList(destinatarios, "Id", "Nome", cartas.UtilizadorDestinatarioFk);
            return View(cartas);
        }


      
        // GET: Cartas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            
            if (id == null) return NotFound();

            var carta = await _context.Cartas
                .Include(c => c.UtilizadorRemetente)
                .Include(c => c.UtilizadorDestinatario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (carta == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);

            if (carta.UtilizadorRemetenteFk != utilizador.Id && carta.UtilizadorDestinatarioFk != utilizador.Id)
                return Forbid();

            return View(carta);
        }

        // POST: Cartas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descricao,Topico,DataEnvio,DataCriacao,UtilizadoresEFk,UtilizadoresDFk")] Cartas cartas)
        {
            
            if (id == null) return NotFound();

            var carta = await _context.Cartas
                .Include(c => c.UtilizadorRemetente)
                .Include(c => c.UtilizadorDestinatario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (carta == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);

            if (carta.UtilizadorRemetenteFk != utilizador.Id && carta.UtilizadorDestinatarioFk != utilizador.Id)
                return Forbid();

            return View(carta);
        }

        // GET: Cartas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var carta = await _context.Cartas
                .Include(c => c.UtilizadorRemetente)
                .Include(c => c.UtilizadorDestinatario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (carta == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);

            if (carta.UtilizadorRemetenteFk != utilizador.Id && carta.UtilizadorDestinatarioFk != utilizador.Id)
                return Forbid();

            return View(carta);
        }

        // POST: Cartas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == null) return NotFound();

            var carta = await _context.Cartas
                .Include(c => c.UtilizadorRemetente)
                .Include(c => c.UtilizadorDestinatario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (carta == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);

            if (carta.UtilizadorRemetenteFk != utilizador.Id && carta.UtilizadorDestinatarioFk != utilizador.Id)
                return Forbid();

            return View(carta);
        }

        private bool CartasExists(int id)
        {
            return _context.Cartas.Any(e => e.Id == id);
        }
    }
}
