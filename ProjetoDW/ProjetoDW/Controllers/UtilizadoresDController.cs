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
    public class UtilizadoresDController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UtilizadoresDController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UtilizadoresD
        public async Task<IActionResult> Index()
        {
            return View(await _context.UtilizadoresD.ToListAsync());
        }

        // GET: UtilizadoresD/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizadoresD = await _context.UtilizadoresD
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizadoresD == null)
            {
                return NotFound();
            }

            return View(utilizadoresD);
        }

        // GET: UtilizadoresD/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UtilizadoresD/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Password,Imagem,Telemovel,Email,NIF,Idade,DataNascimento")] UtilizadoresD utilizadoresD)
        {
            if (ModelState.IsValid)
            {
                _context.Add(utilizadoresD);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(utilizadoresD);
        }

        // GET: UtilizadoresD/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizadoresD = await _context.UtilizadoresD.FindAsync(id);
            if (utilizadoresD == null)
            {
                return NotFound();
            }
            return View(utilizadoresD);
        }

        // POST: UtilizadoresD/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Password,Imagem,Telemovel,Email,NIF,Idade,DataNascimento")] UtilizadoresD utilizadoresD)
        {
            if (id != utilizadoresD.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utilizadoresD);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadoresDExists(utilizadoresD.Id))
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
            return View(utilizadoresD);
        }

        // GET: UtilizadoresD/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizadoresD = await _context.UtilizadoresD
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizadoresD == null)
            {
                return NotFound();
            }

            return View(utilizadoresD);
        }

        // POST: UtilizadoresD/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilizadoresD = await _context.UtilizadoresD.FindAsync(id);
            if (utilizadoresD != null)
            {
                _context.UtilizadoresD.Remove(utilizadoresD);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilizadoresDExists(int id)
        {
            return _context.UtilizadoresD.Any(e => e.Id == id);
        }
    }
}
