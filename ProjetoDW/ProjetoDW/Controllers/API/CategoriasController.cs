using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoDW.Data;
using ProjetoDW.Models;
using ProjetoDW.Models.ViewModels;

namespace ProjetoDW.Controllers.API
{
   [Route("api/[controller]")]
   [ApiController]


public class CategoriasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

      public CategoriasController(ApplicationDbContext context)
      {
         _context = context;
      }

      // GET: api/Categorias
      [HttpGet]

      public async Task<List<CategoriasDTO>> GetCategorias()
      {
         // o que tínhamos
         // // SELECT *
         // // FROM Utilizadores
         // return await _context.Fotografias.ToListAsync();

         // o que pretendemos...
         // SELECT Id,Nome,UtilizadorCriadorID,UtilizadorCriador
         // FROM Categorias
         
         var listaCategorias = await _context.Categorias
            .OrderByDescending(f => f.Id)
            .Select(f => new CategoriasDTO()
            {
               Id = f.Id,
               Nome = f.Nome,
               UtilizadorCriadorId = f.UtilizadorCriadorId
            })
            .ToListAsync();
         return listaCategorias;
      }

      // GET: api/Categorias/5
      [HttpGet("{id}")]
      public async Task<ActionResult<CategoriasDTO>> GetCategoriasID(int id)
      {
         var categorias = await _context.Categorias
            .Where(f => f.Id == id)
            .Select(f => new CategoriasDTO()
            {
               Id = f.Id,
               Nome = f.Nome,
               UtilizadorCriadorId = f.UtilizadorCriadorId
            })
            .FirstOrDefaultAsync();

         if (categorias == null)
         {
            return NotFound();
         }

         return categorias;
      }

      // PUT: api/Categorias/5
      // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      [HttpPut("{id}")]
      public async Task<IActionResult> PutCategorias(int id, Categorias categoria)
      {
         if (id != categoria.Id)
         {
            return BadRequest();
         }

         _context.Entry(categoria).State = EntityState.Modified;
         try
         {
            await _context.SaveChangesAsync();
         }
         catch (DbUpdateConcurrencyException)
         {
            if (!CategoriaExiste(id))
            {
               return NotFound();
            }
            else
            {
               throw;
            }
         }

         return NoContent();
      }

      // POST: api/Categorias
      // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      [HttpPost]
      public async Task<ActionResult<Categorias>> PostCategoria(Categorias categoria)
      {
         _context.Categorias.Add(categoria);
         await _context.SaveChangesAsync();

         return Ok("Categoria Criada Com Sucesso!");
      }

      // DELETE: api/Categorias/5
      [HttpDelete("{id}")]

      public async Task<IActionResult> DeleteCategoria(int id)
      {
         var categoria= await _context.Categorias.FindAsync(id);
         if (categoria == null)
         {

            return NotFound();
         }

         _context.Categorias.Remove(categoria);
         await _context.SaveChangesAsync();

         return NoContent();
      }


      private bool CategoriaExiste(int id)
      {
         return _context.Categorias.Any(e => e.Id == id);
      }
}
}