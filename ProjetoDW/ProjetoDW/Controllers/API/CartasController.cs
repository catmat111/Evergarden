using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppFotos.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoDW.Data;
using ProjetoDW.Models;
using ProjetoDW.Models.ViewModels;

namespace ProjetoDW.Controllers.API {
   [Route("api/[controller]")]
   [ApiController]
   public class CartasController:ControllerBase {
      private readonly ApplicationDbContext _context;

      public CartasController(ApplicationDbContext context) {
         _context=context;
      }

      // GET: api/Cartas
      [HttpGet]

      public async Task<ActionResult<IEnumerable<CartasDTO>>> GetCartas() {
         // o que tínhamos
         // // SELECT *
         // // FROM Cartas
         // return await _context.Fotografias.ToListAsync();

         // o que pretendemos...
         // SELECT Titulo, Descricao, Data de Envio, Remetente e Destinatário
         // FROM Cartas

         var listagemFotos = await _context.Cartas
                                           .OrderByDescending(f => f.DataEnvio)
                                           .Select(f => new CartasDTO {
                                              Titulo=f.Titulo,
                                              Descricao=f.Descricao,
                                              DataEnvio= f.DataEnvio,
                                              UtilizadorRemetenteFk=f.UtilizadorRemetenteFk,
                                              UtilizadorDestinatarioFk = f.UtilizadorDestinatarioFk
                                           })
                                           .ToListAsync();
         return listagemFotos;
      }

      // GET: api/Cartas/5
      [HttpGet("{id}")]
      public async Task<ActionResult<CartasDTO>> GetFotografia(int id) {
         var cartas = await _context.Cartas
                                        .Where(f => f.Id==id)
                                        .Select(f => new CartasDTO {
                                           Titulo=f.Titulo,
                                           Descricao=f.Descricao,
                                           DataEnvio= f.DataEnvio,
                                           UtilizadorRemetenteFk=f.UtilizadorRemetenteFk,
                                           UtilizadorDestinatarioFk = f.UtilizadorDestinatarioFk
                                        })
                                        .FirstOrDefaultAsync();

         if (cartas==null) {
            return NotFound();
         }

         return cartas;
      }

      // PUT: api/Cartas/5
      // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      [HttpPut("{id}")]
      public async Task<IActionResult> PutFotografia(int id,Cartas cartas) {
         if (id!=cartas.Id) {
            return BadRequest();
         }

         _context.Entry(cartas).State=EntityState.Modified;
         try {
            await _context.SaveChangesAsync();
         }
         catch (DbUpdateConcurrencyException) {
            if (!CartasExiste(id)) {
               return NotFound();
            }
            else {
               throw;
            }
         }

         return NoContent();
      }

      // POST: api/Cartas
      // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      [HttpPost]
      public async Task<ActionResult<Cartas>> PostFotografia(Cartas cartas) {
         _context.Cartas.Add(cartas);
         await _context.SaveChangesAsync();

         return CreatedAtAction("GetCartas",new { id = cartas.Id },cartas);
      }

      // DELETE: api/Cartas/5
      [HttpDelete("{id}")]

      public async Task<IActionResult> DeleteFotografia(int id) {
         var cartas = await _context.Cartas.FindAsync(id);
         if (cartas==null) {

            return NotFound();
         }

         _context.Cartas.Remove(cartas);
         await _context.SaveChangesAsync();

         return NoContent();
      }


      private bool CartasExiste(int id) {
         return _context.Cartas.Any(e => e.Id==id);
      }
   }
   
}
