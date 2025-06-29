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
    /// <summary>
    /// Controlador para gerir as operações relacionadas com as Tarefas (To-Do list).
    /// Todas as ações neste controlador requerem que o utilizador esteja autenticado.
    /// As tarefas são privadas para cada utilizador.
    /// </summary>
    [Authorize] // Garante que apenas utilizadores autenticados podem aceder a este controlador.
    public class TarefaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="TarefaController"/>.
        /// </summary>
        /// <param name="context">O contexto da base de dados da aplicação.</param>
        /// <param name="userManager">O serviço para gestão de utilizadores do Identity.</param>
        public TarefaController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tarefa
        /// <summary>
        /// Apresenta a lista de tarefas pertencentes ao utilizador autenticado.
        /// Esta ação não é chamada diretamente, pois a lista de tarefas é mostrada na View 'Index' de 'CartasController'.
        /// </summary>
        /// <returns>Uma View com a lista de tarefas do utilizador.</returns>
        public async Task<IActionResult> Index()
        {
            // Obtém o ID do utilizador autenticado.
            var userId = _userManager.GetUserId(User);

            // Procura na base de dados todas as tarefas associadas a este ID de utilizador.
            var tarefas = await _context.Tarefa
                .Where(t => t.UtilizadorId == userId)
                .ToListAsync();
            
            // Retorna a view com a lista de tarefas.
            return View(tarefas); 
        }

        // POST: Tarefa/Create
        /// <summary>
        /// Cria uma nova tarefa para o utilizador autenticado.
        /// Após a criação, redireciona para a página principal das cartas.
        /// </summary>
        /// <param name="tarefa">O objeto Tarefa com os dados do formulário.</param>
        /// <returns>Redireciona para a ação Index do CartasController.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tarefa tarefa)
        {
            var userId = _userManager.GetUserId(User); 

            // Valida se o nome da tarefa foi preenchido e se o utilizador está autenticado.
            if (!string.IsNullOrEmpty(tarefa.Nome) && !string.IsNullOrEmpty(userId))
            {
                // Define as propriedades da nova tarefa.
                tarefa.UtilizadorId = userId;
                tarefa.Terminado = false; // Novas tarefas começam como não terminadas.

                // Adiciona a tarefa ao contexto e guarda as alterações na base de dados.
                _context.Add(tarefa);
                await _context.SaveChangesAsync();
            }
            
            // Redireciona o utilizador para a página principal das cartas, onde a lista de tarefas é exibida.
            return RedirectToAction("Index", "Cartas");
        }

        // GET: Tarefa/Edit/5
        /// <summary>
        /// Apresenta o formulário de edição para uma tarefa específica.
        /// Garante que apenas o proprietário da tarefa a pode editar.
        /// </summary>
        /// <param name="id">O ID da tarefa a ser editada.</param>
        /// <returns>A View de edição ou um resultado de erro (NotFound, Unauthorized).</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tarefa = await _context.Tarefa.FindAsync(id);
            
            // Verifica se a tarefa existe e se pertence ao utilizador atual.
            if (tarefa == null || tarefa.UtilizadorId != _userManager.GetUserId(User))
                return Unauthorized(); // Se não for o dono, não tem autorização.

            return View(tarefa);
        }

        // POST: Tarefa/Toggle/5
        /// <summary>
        /// Alterna o estado de conclusão (Terminado/Não Terminado) de uma tarefa.
        /// </summary>
        /// <param name="id">O ID da tarefa a ser alterada.</param>
        /// <returns>Redireciona para a ação Index do CartasController.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var tarefa = await _context.Tarefa.FindAsync(id);
            if (tarefa != null)
            {
                // Inverte o valor booleano da propriedade 'Terminado'.
                tarefa.Terminado = !tarefa.Terminado;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Cartas");
        }

        // GET: Tarefa/Delete/5
        /// <summary>
        /// Apresenta a página de confirmação para apagar uma tarefa.
        /// </summary>
        /// <param name="id">O ID da tarefa a ser apagada.</param>
        /// <returns>A View de confirmação de eliminação ou um resultado de erro.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            // Procura a tarefa garantindo que pertence ao utilizador autenticado.
            var tarefa = await _context.Tarefa.FirstOrDefaultAsync(t => t.Id == id && t.UtilizadorId == userId);

            if (tarefa == null) return Unauthorized(); // Não encontrado ou não pertence ao utilizador.

            return View(tarefa);
        }

        // POST: Tarefa/Delete/5
        /// <summary>
        /// Confirma e executa a eliminação de uma tarefa.
        /// </summary>
        /// <param name="id">O ID da tarefa a ser eliminada.</param>
        /// <returns>Redireciona para a ação Index do CartasController.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Idealmente, este método deveria chamar-se "DeleteConfirmed" e ter [ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var tarefa = await _context.Tarefa.FindAsync(id);
            if (tarefa != null)
            {
                // Remove a tarefa do contexto e guarda as alterações.
                _context.Tarefa.Remove(tarefa);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Cartas");
        }
        
        /// <summary>
        /// Método privado para verificar a existência de uma tarefa pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da tarefa.</param>
        /// <returns>Verdadeiro se a tarefa existe, falso caso contrário.</returns>
        private bool TarefaExists(int id)
        {
            return _context.Tarefa.Any(e => e.Id == id);
        }

        /// <summary>
        /// Endpoint de API para adicionar uma nova tarefa via AJAX.
        /// Usado para adicionar tarefas sem recarregar a página inteira.
        /// </summary>
        /// <param name="nome">O nome da nova tarefa.</param>
        /// <returns>Um resultado JSON indicando sucesso e os dados da tarefa criada.</returns>
        [HttpPost]
        public async Task<IActionResult> Adicionar(string nome)
        {
            var userId = _userManager.GetUserId(User);

            // Cria uma nova instância de Tarefa
            var tarefa = new Tarefa
            {
                Nome = nome,
                Terminado = false,
                UtilizadorId = userId
            };

            // Adiciona e guarda na base de dados
            _context.Add(tarefa);
            await _context.SaveChangesAsync();

            // Retorna uma resposta JSON para ser processada pelo JavaScript no cliente.
            return Json(new { sucesso = true, tarefa = new { nome = tarefa.Nome, terminado = tarefa.Terminado } });
        }
    }
}