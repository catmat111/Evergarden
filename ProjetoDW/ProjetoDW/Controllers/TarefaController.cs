using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoDW.Data;
using ProjetoDW.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ProjetoDW.Controllers
{
    [Authorize]
    public class TarefaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TarefaController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tarefa
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var tarefas = await _context.Tarefa
                .Where(t => t.UtilizadorId == userId)
                .ToListAsync();

            return View(tarefas); // <-- agora passa o modelo corretamente
        }


        // POST: Tarefa/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tarefa tarefa)
        {
            var userId = _userManager.GetUserId(User); // Certifica-te que _userManager está injetado

            if (!string.IsNullOrEmpty(tarefa.Nome) && !string.IsNullOrEmpty(userId))
            {
                tarefa.UtilizadorId = userId;
                tarefa.Terminado = false;

                _context.Add(tarefa);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Cartas");
        }



        // GET: Tarefa/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tarefa = await _context.Tarefa.FindAsync(id);
            if (tarefa == null || tarefa.UtilizadorId != _userManager.GetUserId(User))
                return Unauthorized();

            return View(tarefa);
        }

        // POST: Tarefa/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var tarefa = await _context.Tarefa.FindAsync(id);
            if (tarefa != null)
            {
                tarefa.Terminado = !tarefa.Terminado;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Cartas");
        }



        // GET: Tarefa/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var tarefa = await _context.Tarefa.FirstOrDefaultAsync(t => t.Id == id && t.UtilizadorId == userId);

            if (tarefa == null) return Unauthorized();

            return View(tarefa);
        }

        // POST: Tarefa/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tarefa = await _context.Tarefa.FindAsync(id);
            if (tarefa != null)
            {
                _context.Tarefa.Remove(tarefa);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Cartas");
        }


        private bool TarefaExists(int id)
        {
            return _context.Tarefa.Any(e => e.Id == id);
        }
        [HttpPost]
        public async Task<IActionResult> Adicionar(string nome)
        {
            var userId = _userManager.GetUserId(User);

            var tarefa = new Tarefa
            {
                Nome = nome,
                Terminado = false,
                UtilizadorId = userId
            };

            _context.Add(tarefa);
            await _context.SaveChangesAsync();

            return Json(new { sucesso = true, tarefa = new { nome = tarefa.Nome, terminado = tarefa.Terminado } });
        }
        
    }
}
