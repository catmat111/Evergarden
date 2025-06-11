using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Index(string searchString)
        {
            var isRemetente = User.IsInRole("Remetente");
            if (isRemetente)
            {


                var user = await _userManager.GetUserAsync(User);
                var query = _context.Cartas
                    .Include(c => c.UtilizadorRemetente)
                    .Include(c => c.UtilizadorDestinatario)
                    .Include(c => c.Categorias)
                    .Where(c => c.UtilizadorRemetente.IdentityUserID ==
                                user.Id); // Mostra só as cartas do remetente autenticado

                if (!string.IsNullOrEmpty(searchString))
                {
                    query = query.Where(c =>
                        c.UtilizadorDestinatario.Nome.Contains(searchString));
                }
                var tarefas = await _context.Tarefa
                    .Where(t => t.UtilizadorId == user.Id)
                    .ToListAsync();
                ViewBag.Tarefas = tarefas;

                return View(await query.ToListAsync());
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                var query = _context.Cartas
                    .Include(c => c.UtilizadorRemetente)
                    .Include(c => c.UtilizadorDestinatario)
                    .Include(c => c.Categorias)
                    .Where(c => c.UtilizadorDestinatario.IdentityUserID ==
                                user.Id); // Mostra só as cartas do remetente autenticado

                if (!string.IsNullOrEmpty(searchString))
                {
                    query = query.Where(c =>
                        c.UtilizadorDestinatario.Nome.Contains(searchString));
                }


                return View(await query.ToListAsync());
            }
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
        [Authorize(Roles = "Remetente")]

        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            

            // Buscar destinatários associados a este remetente, se necessário
            var remetente = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.IdentityUserID == user.Id);

            var destinatarios = await _context.Utilizadores
                .Where(d => d.RemetenteId == remetente.Id)
                .ToListAsync();

            var categorias = await _context.Categorias.Where(d => d.UtilizadorCriador.Id == user.Id).ToListAsync();
            
            /*var categorias = await _context.Categorias
                .Where(c => c.UtilizadorCriador.IdentityUserID == user.Id)
                .ToListAsync();*/



            ViewBag.UtilizadoresDFk = new SelectList(destinatarios, "Id", "Nome");
            ViewBag.Categorias = categorias;

            
            return View();
            
        }

        // POST: Cartas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cartas carta, List<int> categoriasSelecionadas, DateOnly? DataEnvio)
        {
            var utilizadorAutenticado = await _userManager.GetUserAsync(User);
            var remetente = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == utilizadorAutenticado.Id);

            if (remetente == null)
            {
                return Unauthorized();
            }
            if (carta.UtilizadorDestinatarioFk == null || carta.UtilizadorDestinatarioFk == 0)
            {
                ModelState.AddModelError("UtilizadorDestinatarioFk", "Tem de ter alguém para enviar a sua carta!");
            }

            // Verifica se há pelo menos uma categoria com TemData == true
            var categoriasCompletas = await _context.Categorias
                .Where(c => categoriasSelecionadas.Contains(c.Id))
                .ToListAsync();

            if (categoriasSelecionadas == null || !categoriasSelecionadas.Any())
            {
                ModelState.AddModelError("categoriasSelecionadas", "Tem de ter uma categoria, no mínimo!");
            }
            
            bool exigeData = categoriasCompletas.Any(c => c.Tipo);

            if (exigeData && !DataEnvio.HasValue)
            {
                ModelState.AddModelError("DataEnvio", "Para quando é que queres enviar a carta?");
            }

            if (ModelState.IsValid)
            {
                carta.UtilizadorRemetenteFk = remetente.Id;

                if (exigeData)
                {
                    carta.DataEnvio = DataEnvio.Value;
                }
                    
                carta.DataCriacao = DateOnly.FromDateTime(DateTime.Now);
                

                // Associar as categorias à carta
                carta.Categorias = categoriasCompletas;

                
                _context.Add(carta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se o ModelState não for válido, recarrega os dados da ViewBag para a view
            ViewBag.Categorias = await _context.Categorias
                .Where(c => c.UtilizadorCriador.Id == utilizadorAutenticado.Id)
                .ToListAsync();

            ViewBag.UtilizadoresDFk = new SelectList(await _context.Utilizadores
                .Where(u => u.RemetenteId == remetente.Id)
                .ToListAsync(), "Id", "Nome", carta.UtilizadorDestinatarioFk);

            return View(carta);
        }





      
        // GET: Cartas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var carta = await _context.Cartas
                .Include(c => c.Categorias)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carta == null) return NotFound();

            // Impede edição se já foi enviada
            if (carta.DataEnvio.HasValue && carta.DataEnvio <= DateOnly.FromDateTime(DateTime.Today))
            {
                TempData["Erro"] = "Esta carta já foi enviada e não pode ser editada.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var remetente = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.IdentityUserID == user.Id);

            if (remetente == null) return Unauthorized();

            var categoriasDisponiveis = await _context.Categorias
                .Where(c => c.UtilizadorCriadorId == remetente.IdentityUserID)
                .ToListAsync();

            var temp = carta.Categorias.Any(c => c.Tipo);
            ViewBag.Categorias = categoriasDisponiveis;

            // Indica se há categorias que exigem DataEnvio
            ViewBag.ExigeData = carta.Categorias.Any(c => c.Tipo);

            return View(carta);
        }

        // POST: Cartas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Cartas carta, int[] categoriasSelecionadas)
{
    if (id != carta.Id) return NotFound();

    // Recarrega a carta original
    var cartaExistente = await _context.Cartas
        .Include(c => c.Categorias)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (cartaExistente == null) return NotFound();

    // Verifica se há categorias que exigem DataEnvio
    var categoriasSelecionadasObjs = await _context.Categorias
        .Where(c => categoriasSelecionadas.Contains(c.Id))
        .ToListAsync();

    bool exigeData = categoriasSelecionadasObjs.Any(c => c.Tipo);

    if (!exigeData)
    {
        carta.DataEnvio = null;
    }

    if (ModelState.IsValid)
    {
        try
        {
            // Atualizar propriedades simples
            cartaExistente.Titulo = carta.Titulo;
            cartaExistente.Descricao = carta.Descricao;
            cartaExistente.DataEnvio = exigeData ? carta.DataEnvio : null;

            // Atualizar categorias
            cartaExistente.Categorias.Clear();
            cartaExistente.Categorias.AddRange(categoriasSelecionadasObjs);

            _context.Update(cartaExistente);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Cartas.Any(c => c.Id == carta.Id))
                return NotFound();
            else
                throw;
        }
    }

    // Recarrega categorias se falhar a validação
    var user = await _userManager.GetUserAsync(User);
    var remetente = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == user.Id);
    var categoriasDisponiveis = await _context.Categorias
        .Where(c => c.Id == remetente.Id)
        .ToListAsync();

    ViewBag.Categorias = new MultiSelectList(categoriasDisponiveis, "Id", "Nome", categoriasSelecionadas);
    ViewBag.ExigeData = exigeData;

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
            var carta = await _context.Cartas
                .Include(c => c.Categorias)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (carta == null)
                return NotFound();

            // ➤ Limpar relações com Categorias (muitos-para-muitos)
            carta.Categorias.Clear();

            // ➤ Guardar a mudança antes de remover
            await _context.SaveChangesAsync();

            // ➤ Agora pode remover a carta com segurança
            _context.Cartas.Remove(carta);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool CartasExists(int id)
        {
            return _context.Cartas.Any(e => e.Id == id);
        }
    }
}
