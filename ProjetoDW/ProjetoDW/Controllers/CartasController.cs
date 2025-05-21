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

            var cartas = await _context.Cartas
                .Include(c => c.UtilizadorRemetente)
                .Include(c => c.UtilizadorDestinatario)
                .ToListAsync();

            return View(cartas);
        }



        // GET: Cartas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartas = await _context.Cartas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartas == null)
            {
                return NotFound();
            }

            return View(cartas);
        }

        // GET: Cartas/Create
        public IActionResult Create()
        {
            ViewData["UtilizadorDestinatarioFk"] = new SelectList(_context.Utilizadores, "Id", "Nome");
            return View();
            
        }

        // POST: Cartas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Descricao,Topico,DataEnvio,UtilizadoresEFk,UtilizadoresDFk")] Cartas cartas)
        {
            if (ModelState.IsValid)
            {
                
                // Preencher automaticamente a DataCriacao com a data atual
                cartas.DataCriacao = DateTime.Now;

                // Adicionar a carta ao contexto e salvar no banco de dados
                _context.Add(cartas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cartas);
        }

      
        // GET: Cartas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartas = await _context.Cartas.FindAsync(id);
            if (cartas == null)
            {
                return NotFound();
            }
            return View(cartas);
        }

        // POST: Cartas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descricao,Topico,DataEnvio,DataCriacao,UtilizadoresEFk,UtilizadoresDFk")] Cartas cartas)
        {
            if (id != cartas.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartasExists(cartas.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cartas);
        }

        // GET: Cartas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartas = await _context.Cartas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartas == null)
            {
                return NotFound();
            }

            return View(cartas);
        }

        // POST: Cartas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartas = await _context.Cartas.FindAsync(id);
            if (cartas != null)
            {
                _context.Cartas.Remove(cartas);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartasExists(int id)
        {
            return _context.Cartas.Any(e => e.Id == id);
        }
    }
}
