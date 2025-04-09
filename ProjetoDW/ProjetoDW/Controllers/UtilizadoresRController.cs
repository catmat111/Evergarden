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
            return View(await _context.UtilizadoresR.ToListAsync());
        }

        // GET: UtilizadoresR/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizadoresR = await _context.UtilizadoresR
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
            return View();
        }

        // POST: UtilizadoresR/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Password,Imagem,Telemovel,Email,NIF,Idade,DataNascimento")] UtilizadoresR utilizadoresR)
        {
            if (ModelState.IsValid)
            {
                _context.Add(utilizadoresR);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(utilizadoresR);
        }

        // GET: UtilizadoresR/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizadoresR = await _context.UtilizadoresR.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Password,Imagem,Telemovel,Email,NIF,Idade,DataNascimento")] UtilizadoresR utilizadoresR)
        {
            if (id != utilizadoresR.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utilizadoresR);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadoresRExists(utilizadoresR.Id))
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
            return View(utilizadoresR);
        }

        // GET: UtilizadoresR/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizadoresR = await _context.UtilizadoresR
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
            var utilizadoresR = await _context.UtilizadoresR.FindAsync(id);
            if (utilizadoresR != null)
            {
                _context.UtilizadoresR.Remove(utilizadoresR);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilizadoresRExists(int id)
        {
            return _context.UtilizadoresR.Any(e => e.Id == id);
        }
    }
}
