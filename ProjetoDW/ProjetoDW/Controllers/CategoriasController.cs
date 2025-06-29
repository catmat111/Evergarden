using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoDW.Data;
using ProjetoDW.Models;

namespace ProjetoDW.Controllers
{
    /// <summary>
    /// Controlador para gerir as operações CRUD (Criar, Ler, Atualizar, Apagar) para as Categorias.
    /// As categorias são criadas e geridas pelos próprios utilizadores.
    /// </summary>
    public class CategoriasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="CategoriasController"/>.
        /// </summary>
        /// <param name="context">O contexto da base de dados da aplicação.</param>
        /// <param name="userManager">O serviço para gestão de utilizadores do Identity.</param>
        public CategoriasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Categorias
        /// <summary>
        /// Apresenta uma lista de todas as categorias criadas pelo utilizador autenticado.
        /// Permite filtrar a lista de categorias por um termo de pesquisa.
        /// </summary>
        /// <param name="searchString">A string de texto para procurar nos nomes das categorias.</param>
        /// <returns>Uma View com a lista de categorias do utilizador.</returns>
        public async Task<IActionResult> Index(string searchString)
        {
            // Obtém o utilizador atualmente autenticado.
            var user = await _userManager.GetUserAsync(User);

            // Cria uma query para buscar as categorias, incluindo o utilizador criador.
            // Filtra para mostrar apenas as categorias criadas pelo utilizador atual.
            var query = _context.Categorias
                .Include(c => c.UtilizadorCriador)
                .Where(c => c.UtilizadorCriador.Id == user.Id);

            // Se uma string de pesquisa for fornecida, filtra as categorias cujo nome a contém.
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Nome.Contains(searchString));
            }

            // Executa a query e obtém a lista de categorias.
            var categoriasDoUtilizador = await query.ToListAsync();
            return View(categoriasDoUtilizador);
        }


        // GET: Categorias/Details/5
        /// <summary>
        /// Mostra os detalhes de uma categoria específica.
        /// </summary>
        /// <param name="id">O ID da categoria a ser visualizada.</param>
        /// <returns>Uma View com os detalhes da categoria ou um resultado NotFound se não for encontrada.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Procura a categoria pelo ID, incluindo a informação do utilizador que a criou.
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
        /// <summary>
        /// Apresenta o formulário para criar uma nova categoria.
        /// </summary>
        /// <returns>Uma View com o formulário de criação.</returns>
        public IActionResult Create()
        {
            return View();
        }


        // POST: Categorias/Create
        /// <summary>
        /// Processa os dados do formulário de criação de uma nova categoria.
        /// Valida os dados e impede a criação de categorias com nomes duplicados.
        /// </summary>
        /// <param name="categorias">O objeto Categorias com os dados do formulário.</param>
        /// <returns>Redireciona para a Index em caso de sucesso; caso contrário, retorna a View de criação com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Proteção contra ataques Cross-Site Request Forgery (CSRF).
        public async Task<IActionResult> Create([Bind("Nome,Tipo")] Categorias categorias)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(); // Utilizador não autenticado.
                }

                // Verifica se já existe uma categoria com o mesmo nome (ignorando maiúsculas/minúsculas).
                var categoriaExistente = await _context.Categorias
                    .FirstOrDefaultAsync(c => c.Nome.ToLower() == categorias.Nome.ToLower());

                if (categoriaExistente != null)
                {
                    // Adiciona um erro ao modelo se o nome já existir.
                    ModelState.AddModelError("Nome", "Já existe uma categoria com este nome");
                    return View(categorias);
                }
                
                // Associa o utilizador criador à nova categoria.
                var user = await _userManager.FindByIdAsync(userId);
                categorias.UtilizadorCriador = user;

                _context.Add(categorias);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(categorias);
        }


        // GET: Categorias/Edit/5
        /// <summary>
        /// Apresenta o formulário para editar uma categoria existente.
        /// </summary>
        /// <param name="id">O ID da categoria a ser editada.</param>
        /// <returns>Uma View com o formulário de edição ou NotFound se a categoria não existir.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            // Procura a categoria na base de dados pelo seu ID.
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }


        // POST: Categorias/Edit/5
        /// <summary>
        /// Processa os dados do formulário de edição de uma categoria.
        /// Atualiza a categoria na base de dados se os dados forem válidos.
        /// </summary>
        /// <param name="id">O ID da categoria a ser atualizada.</param>
        /// <param name="categoriaAtualizada">O objeto Categorias com os dados atualizados.</param>
        /// <returns>Redireciona para a Index em caso de sucesso; caso contrário, retorna a View de edição com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo,Nome")] Categorias categoriaAtualizada)
        {
            if (!ModelState.IsValid)
            {
                return View(categoriaAtualizada);
            }

            // Obtém a categoria original da BD sem a rastrear para evitar conflitos.
            var categoriaExistente = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoriaExistente == null)
            {
                return NotFound();
            }

            // Garante que o ID do criador original é preservado.
            categoriaAtualizada.UtilizadorCriadorId = categoriaExistente.UtilizadorCriadorId;

            try
            {
                // Marca a entidade como modificada para que o EF a atualize.
                _context.Update(categoriaAtualizada);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                // Trata exceções de concorrência (ex: outro utilizador apagou o registo).
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
        /// <summary>
        /// Apresenta a página de confirmação para apagar uma categoria.
        /// </summary>
        /// <param name="id">O ID da categoria a ser apagada.</param>
        /// <returns>Uma View com os dados da categoria para confirmação.</returns>
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
        /// <summary>
        /// Confirma e executa a eliminação de uma categoria.
        /// Remove as associações desta categoria de todas as cartas antes de a apagar.
        /// </summary>
        /// <param name="id">O ID da categoria a ser eliminada.</param>
        /// <returns>Uma View de sucesso "CategoriaDeletada".</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Encontra a categoria e inclui as cartas associadas a ela.
            var categoria = await _context.Categorias
                .Include(c => c.Cartas)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            // Itera sobre uma cópia da lista de cartas associadas.
            // Remove a referência a esta categoria de cada carta.
            // Isto é crucial para quebrar a relação muitos-para-muitos.
            foreach (var carta in categoria.Cartas.ToList())
            {
                carta.Categorias.Remove(categoria);
            }

            // Guarda as alterações nas cartas (a remoção da associação).
            await _context.SaveChangesAsync();

            // Agora que a relação foi quebrada, elimina a categoria.
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            // Retorna uma view para indicar que a categoria foi eliminada com sucesso.
            return View("CategoriaDeletada");
        }

        /// <summary>
        /// Verifica se uma categoria com um determinado ID existe na base de dados.
        /// </summary>
        /// <param name="id">O ID da categoria a verificar.</param>
        /// <returns>True se a categoria existir, False caso contrário.</returns>
        private bool CategoriasExists(int id)
        {
            return _context.Categorias.Any(e => e.Id == id);
        }
    }
}