using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoDW.Data;
using ProjetoDW.Models;

namespace ProjetoDW.Controllers
{
    public class UtilizadoresRController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UtilizadoresRController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UtilizadoresR
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utilizadores.ToListAsync());
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
        public IActionResult Create()
        {
            var utilizador = new Utilizadores();
            return View(utilizador);
        }

        // POST: UtilizadoresR/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Password,Imagem,Telemovel,Email,NIF,Idade,DataNascimento")] Utilizadores utilizadores)
        {
            // Verifica se foi submetido algum ficheiro
            if (utilizadores.Imagem != null && utilizadores.Imagem.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                // Cria a pasta se não existir
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Cria nome único para o ficheiro
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(utilizadores.Imagem.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Guarda o ficheiro no servidor
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await utilizadores.Imagem.CopyToAsync(stream);
                }

                // Guarda o caminho relativo na base de dados
                utilizadores.ImagemPath = "/uploads/" + fileName;
            }

            if (ModelState.IsValid)
            {
                _context.Add(utilizadores);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(utilizadores);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Password,Imagem,Telemovel,Email,NIF,Idade,DataNascimento")] Utilizadores utilizadores)
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
                return RedirectToAction(nameof(Index));
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
