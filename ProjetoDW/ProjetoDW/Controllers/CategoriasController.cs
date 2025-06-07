using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using ProjetoDW.Data;
using ProjetoDW.Models;

namespace ProjetoDW.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CategoriasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Categorias
        public async Task<IActionResult> Index(string searchString)
        {
            var user = await _userManager.GetUserAsync(User);

            var query = _context.Categorias
                .Include(c => c.UtilizadorCriador)
                .Where(c => c.UtilizadorCriador.Id == user.Id);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Nome.Contains(searchString));
            }

            var categoriasDoUtilizador = await query.ToListAsync();
            return View(categoriasDoUtilizador);
        }


        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorias = await _context.Categorias
                .Include(c => c.UtilizadorCriador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categorias == null)
            {
                return NotFound();
            }

            return View(categorias);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: Categorias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Tipo")] Categorias categorias)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized();
                }

                // Buscar o IdentityUser completo
                var user = await _userManager.FindByIdAsync(userId);
                categorias.UtilizadorCriador = user;

                _context.Add(categorias);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(categorias);
        }


        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }



        // POST: Categorias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo,Nome")] Categorias categoriaAtualizada)
        {
            if (!ModelState.IsValid)
            {
                return View(categoriaAtualizada);
            }

            var categoriaExistente = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoriaExistente == null)
            {
                return NotFound();
            }

            // Preserva o UtilizadorCriadorId original
            categoriaAtualizada.UtilizadorCriadorId = categoriaExistente.UtilizadorCriadorId;

            try
            {
                _context.Update(categoriaAtualizada);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriasExists(categoriaAtualizada.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorias = await _context.Categorias
                .Include(c => c.UtilizadorCriador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categorias == null)
            {
                return NotFound();
            }

            return View(categorias);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Cartas)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

// Remover esta categoria da lista de cada carta associada
            foreach (var carta in categoria.Cartas.ToList())
            {
                carta.Categorias.Remove(categoria);
            }

// Guardar alterações antes de apagar
            await _context.SaveChangesAsync();

// Agora eliminar a categoria
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return View("CategoriaDeletada");

            return View("CategoriaDeletada");
        }

        private bool CategoriasExists(int id)
        {
            return _context.Categorias.Any(e => e.Id == id);
        }
    }
}
