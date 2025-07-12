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
   public class UtilizadoresController : ControllerBase
   {
      private readonly ApplicationDbContext _context;

      public UtilizadoresController(ApplicationDbContext context)
      {
         _context = context;
      }

      // GET: api/Utilizadores
      [HttpGet]

      public async Task<List<UtilizadoresDTO>> GetUtilizadores()
      {
         // o que tínhamos
         // // SELECT *
         // // FROM Utilizadores
         // return await _context.Fotografias.ToListAsync();

         // o que pretendemos...
         // SELECT Id, Nome, ImagemPath, Telemovel, Email, DataNascimento
         // FROM Utilizadores
         
         var listagemFotos = await _context.Utilizadores
            .OrderByDescending(f => f.Id)
            .Select(f => new UtilizadoresDTO()
            {
               Id = f.Id,
               Nome = f.Nome,
               ImagemPath = f.ImagemPath,
               Telemovel = f.Telemovel,
               Email = f.Email,
               DataNascimento = f.DataNascimento
            })
            .ToListAsync();
         return listagemFotos;
      }

      // GET: api/Utilizadores/5
      [HttpGet("{id}")]
      public async Task<ActionResult<UtilizadoresDTO>> GetUtilizadoresID(int id)
      {
         var utilizador = await _context.Utilizadores
            .Where(f => f.Id == id)
            .Select(f => new UtilizadoresDTO()
            {
               Id = f.Id,
               Nome = f.Nome,
               ImagemPath = f.ImagemPath,
               Telemovel = f.Telemovel,
               Email = f.Email,
               DataNascimento = f.DataNascimento
            })
            .FirstOrDefaultAsync();

         if (utilizador == null)
         {
            return NotFound();
         }

         return utilizador;
      }

      // PUT: api/Utilizadores/5
      // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      [HttpPut("{id}")]
      public async Task<IActionResult> PutUtilizador(int id, Utilizadores utilizador)
      {
         if (id != utilizador.Id)
         {
            return BadRequest();
         }

         _context.Entry(utilizador).State = EntityState.Modified;
         try
         {
            await _context.SaveChangesAsync();
         }
         catch (DbUpdateConcurrencyException)
         {
            if (!UtilizadorExiste(id))
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

      // POST: api/Utilizadores
      // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      [HttpPost]
      public async Task<ActionResult<Utilizadores>> PostUtilizador(Utilizadores utilizador)
      {
         _context.Utilizadores.Add(utilizador);
         await _context.SaveChangesAsync();

         return Ok("Utilizador Criado Com Sucesso!");
      }

      // DELETE: api/Utilizadores/5
      [HttpDelete("{id}")]

      public async Task<IActionResult> DeleteUtilizador(int id)
      {
         var utilizador= await _context.Utilizadores.FindAsync(id);
         if (utilizador== null)
         {

            return NotFound();
         }

         _context.Utilizadores.Remove(utilizador);
         await _context.SaveChangesAsync();

         return NoContent();
      }


      private bool UtilizadorExiste(int id)
      {
         return _context.Utilizadores.Any(e => e.Id == id);
      }
   }
}