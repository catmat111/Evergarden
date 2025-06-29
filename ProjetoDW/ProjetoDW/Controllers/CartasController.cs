using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoDW.Data;
using ProjetoDW.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using ProjetoDW.Services;

namespace ProjetoDW.Controllers
{
    /// <summary>
    /// Controlador para gerir as operações CRUD (Criar, Ler, Atualizar, Apagar) para as Cartas.
    /// Gere a visualização, criação, edição e eliminação de cartas,
    /// aplicando regras específicas para Remetentes e Destinatários.
    /// </summary>
    public class CartasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHubContext<SignarRNotificacao> _hubContext;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="CartasController"/>.
        /// </summary>
        /// <param name="context">O contexto da base de dados da aplicação.</param>
        /// <param name="userManager">O serviço para gestão de utilizadores do Identity.</param>
        public CartasController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHubContext<SignarRNotificacao> _hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = _hubContext;
        }

        // GET: Cartas
        /// <summary>
        /// Apresenta uma lista de cartas para o utilizador autenticado.
        /// A lista é filtrada com base no papel do utilizador (Remetente ou Destinatário).
        /// Permite filtrar as cartas por nome do destinatário/remetente e por data de envio.
        /// </summary>
        /// <param name="searchDestinatario">String para pesquisar no nome do destinatário.</param>
        /// <param name="searchRemetente">String para pesquisar no nome do remetente.</param>
        /// <param name="dataSelecionada">Data específica para filtrar as cartas enviadas.</param>
        /// <returns>Uma View com a lista de cartas filtrada.</returns>
        public async Task<IActionResult> Index(string searchDestinatario, string searchRemetente, DateTime? dataSelecionada)
        {
            // Obtém o utilizador autenticado
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(); // Retorna não autorizado se não houver utilizador

            // Verifica se o utilizador tem o papel "Remetente"
            bool isRemetente = User.IsInRole("Remetente");
            
            // Inicia a query para obter as cartas, incluindo dados relacionados dos remetentes, destinatários e categorias
            IQueryable<Cartas> query = _context.Cartas
                .Include(c => c.UtilizadorRemetente)
                .Include(c => c.UtilizadorDestinatario)
                .Include(c => c.Categorias)
                .AsQueryable();

            if (isRemetente)
            {
                // Se for Remetente, filtra as cartas onde ele é o remetente
                query = query.Where(c => c.UtilizadorRemetente.IdentityUserID == user.Id);

                // Aplica filtro de pesquisa por nome do destinatário, se fornecido
                if (!string.IsNullOrEmpty(searchDestinatario))
                {
                    query = query.Where(c => c.UtilizadorDestinatario.Nome.Contains(searchDestinatario));
                }
            }
            else
            {
                // Se for Destinatário, filtra as cartas onde ele é o destinatário
                query = query.Where(c => c.UtilizadorDestinatario.IdentityUserID == user.Id);

                // Aplica filtro de pesquisa por nome do remetente, se fornecido
                if (!string.IsNullOrEmpty(searchRemetente))
                {
                    query = query.Where(c => c.UtilizadorRemetente.Nome.Contains(searchRemetente));
                }
            }

            // Aplica filtro por data de envio, se uma data for selecionada
            if (dataSelecionada.HasValue)
            {
                query = query.Where(c => c.DataEnvio.HasValue && c.DataEnvio.Value == DateOnly.FromDateTime(dataSelecionada.Value));
            }

            // Obtém as tarefas do utilizador para exibição na View
            var tarefas = await _context.Tarefa
                .Where(t => t.UtilizadorId == user.Id)
                .ToListAsync();

            // Passa dados para a View através do ViewBag
            ViewBag.Tarefas = tarefas;
            ViewBag.DataSelecionada = dataSelecionada?.ToString("yyyy-MM-dd");

            var cartas = await query.ToListAsync();

            // Obtém as datas únicas em que existem cartas para realçar no calendário
            var datasComCartas = cartas
                .Where(c => c.DataEnvio.HasValue)
                .Select(c => c.DataEnvio.Value.ToString("yyyy-MM-dd"))
                .Distinct()
                .ToList();

            ViewBag.DatasComCartas = datasComCartas;

            // Retorna a View "Index" com a lista de cartas
            return View(cartas);
        }


        // GET: Cartas/Details/5
        /// <summary>
        /// Mostra os detalhes de uma carta específica.
        /// Apenas o remetente ou o destinatário da carta podem ver os detalhes.
        /// </summary>
        /// <param name="id">O ID da carta a ser visualizada.</param>
        /// <returns>Uma View com os detalhes da carta ou um resultado de erro (NotFound, Forbid).</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Procura a carta na base de dados, incluindo remetente e destinatário
            var carta = await _context.Cartas
                .Include(c => c.UtilizadorRemetente)
                .Include(c => c.UtilizadorDestinatario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (carta == null) return NotFound();

            // Verifica as permissões do utilizador
            var userId = _userManager.GetUserId(User);
            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);

            // Se o utilizador não for o remetente nem o destinatário, proíbe o acesso
            if (carta.UtilizadorRemetenteFk != utilizador.Id && carta.UtilizadorDestinatarioFk != utilizador.Id)
                return Forbid();

            return View(carta);
        }

        // GET: Cartas/Create
        /// <summary>
        /// Apresenta o formulário para criar uma nova carta.
        /// Apenas utilizadores com o papel "Remetente" podem aceder.
        /// </summary>
        /// <returns>Uma View com o formulário de criação de carta.</returns>
        [Authorize(Roles = "Remetente")] // Restringe o acesso a utilizadores com o papel "Remetente"
        public async Task<IActionResult> Create()
        {
            // Obtém o utilizador autenticado e o seu perfil de remetente
            var user = await _userManager.GetUserAsync(User);
            var remetente = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.IdentityUserID == user.Id);

            // Obtém a lista de destinatários associados a este remetente
            var destinatarios = await _context.Utilizadores
                .Where(d => d.RemetenteId == remetente.Id)
                .ToListAsync();

            // Obtém as categorias criadas pelo remetente
            var categorias = await _context.Categorias
                .Where(d => d.UtilizadorCriadorId == user.Id)
                .ToListAsync();

            // Passa a lista de destinatários e categorias para a View
            ViewBag.UtilizadoresDFk = new SelectList(destinatarios, "Id", "Nome");
            ViewBag.Categorias = categorias;

            return View();
        }

        // POST: Cartas/Create
        /// <summary>
        /// Processa os dados do formulário de criação de uma nova carta.
        /// Valida os dados e, se válidos, guarda a nova carta na base de dados.
        /// </summary>
        /// <param name="carta">O objeto Carta com os dados do formulário.</param>
        /// <param name="categoriasSelecionadas">Lista de IDs das categorias selecionadas.</param>
        /// <param name="DataEnvio">A data de envio opcional da carta.</param>
        /// <returns>Redireciona para a Index em caso de sucesso; caso contrário, retorna a View de criação com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF
        public async Task<IActionResult> Create(Cartas carta, List<int> categoriasSelecionadas, DateOnly? DataEnvio)
        {
            var utilizadorAutenticado = await _userManager.GetUserAsync(User);
            var remetente = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == utilizadorAutenticado.Id);

            if (remetente == null) return Unauthorized();

            // Validações
            if (carta.UtilizadorDestinatarioFk == null || carta.UtilizadorDestinatarioFk == 0)
                ModelState.AddModelError("UtilizadorDestinatarioFk", "Tem de ter alguém para enviar a sua carta!");

            if (categoriasSelecionadas == null || !categoriasSelecionadas.Any())
                ModelState.AddModelError("categoriasSelecionadas", "Tem de ter uma categoria, no mínimo!");

            // Verifica se alguma categoria selecionada exige uma data de envio
            var categoriasCompletas = await _context.Categorias
                .Where(c => categoriasSelecionadas.Contains(c.Id))
                .ToListAsync();
            bool exigeData = categoriasCompletas.Any(c => c.Tipo); // 'Tipo' a true significa que exige data

            if (exigeData && !DataEnvio.HasValue)
                ModelState.AddModelError("DataEnvio", "Para quando é que queres enviar a carta?");

            // Se o modelo for válido, guarda a carta
            if (ModelState.IsValid)
            {
                carta.UtilizadorRemetenteFk = remetente.Id;

                if (exigeData)
                    carta.DataEnvio = DataEnvio.Value;

                carta.DataCriacao = DateOnly.FromDateTime(DateTime.Now);
                carta.Categorias = categoriasCompletas; // Associa as categorias

                _context.Add(carta);
                await _context.SaveChangesAsync();
                await _hubContext.Clients
                    .User(carta.UtilizadorDestinatario.IdentityUserID) // IdentityUserId do destinatário
                    .SendAsync("NovaCartaRecebida", new {
                        id = carta.Id,
                        titulo = carta.Titulo,
                        data = carta.DataCriacao.ToString("dd/MM/yyyy")
                    });
                return RedirectToAction(nameof(Index));
            }

            // Se o modelo for inválido, prepara os dados novamente para a View
            ViewBag.Categorias = await _context.Categorias
                .Where(c => c.UtilizadorCriadorId == utilizadorAutenticado.Id)
                .ToListAsync();

            ViewBag.UtilizadoresDFk = new SelectList(await _context.Utilizadores
                .Where(u => u.RemetenteId == remetente.Id)
                .ToListAsync(), "Id", "Nome", carta.UtilizadorDestinatarioFk);

            return View(carta);
        }

        // GET: Cartas/Edit/5
        /// <summary>
        /// Apresenta o formulário para editar uma carta existente.
        /// Não permite a edição de cartas que já foram enviadas.
        /// </summary>
        /// <param name="id">O ID da carta a ser editada.</param>
        /// <returns>Uma View com o formulário de edição ou um resultado de erro.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var carta = await _context.Cartas
                .Include(c => c.Categorias)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carta == null) return NotFound();

            // Validação: Impede a edição de cartas já enviadas (data de envio no passado ou hoje)
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

            // Carrega as categorias disponíveis para o remetente
            var categoriasDisponiveis = await _context.Categorias
                .Where(c => c.UtilizadorCriadorId == remetente.IdentityUserID)
                .ToListAsync();

            ViewBag.Categorias = categoriasDisponiveis;
            ViewBag.ExigeData = carta.Categorias.Any(c => c.Tipo); // Informa a View se a data é obrigatória

            return View(carta);
        }

        // POST: Cartas/Edit/5
        /// <summary>
        /// Processa os dados do formulário de edição de uma carta.
        /// Atualiza a carta na base de dados se os dados forem válidos.
        /// </summary>
        /// <param name="id">O ID da carta a ser atualizada.</param>
        /// <param name="carta">O objeto Carta com os dados atualizados do formulário.</param>
        /// <param name="categoriasSelecionadas">A nova lista de IDs de categorias para a carta.</param>
        /// <returns>Redireciona para a Index em caso de sucesso; caso contrário, retorna a View de edição com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cartas carta, int[] categoriasSelecionadas)
        {
            if (id != carta.Id) return NotFound();

            var cartaExistente = await _context.Cartas
                .Include(c => c.Categorias)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cartaExistente == null) return NotFound();

            // Obtém os objetos completos das categorias selecionadas
            var categoriasSelecionadasObjs = await _context.Categorias
                .Where(c => categoriasSelecionadas.Contains(c.Id))
                .ToListAsync();

            bool exigeData = categoriasSelecionadasObjs.Any(c => c.Tipo);

            // Se as novas categorias não exigem data, remove a data de envio
            if (!exigeData)
                carta.DataEnvio = null;

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualiza os campos da carta existente
                    cartaExistente.Titulo = carta.Titulo;
                    cartaExistente.Descricao = carta.Descricao;
                    cartaExistente.DataEnvio = exigeData ? carta.DataEnvio : null;

                    // Atualiza a relação com as categorias
                    cartaExistente.Categorias.Clear();
                    cartaExistente.Categorias.AddRange(categoriasSelecionadasObjs);

                    _context.Update(cartaExistente);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Trata exceções de concorrência (ex: outro utilizador apagou o registo)
                    if (!_context.Cartas.Any(c => c.Id == carta.Id))
                        return NotFound();
                    else
                        throw;
                }
            }
            
            // Se o modelo for inválido, prepara os dados novamente para a View de edição
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
        /// <summary>
        /// Apresenta a página de confirmação para apagar uma carta.
        /// </summary>
        /// <param name="id">O ID da carta a ser apagada.</param>
        /// <returns>Uma View com os dados da carta para confirmação ou um resultado de erro.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var carta = await _context.Cartas
                .Include(c => c.UtilizadorRemetente)
                .Include(c => c.UtilizadorDestinatario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (carta == null) return NotFound();
            
            // Verifica as permissões: apenas remetente ou destinatário podem apagar
            var userId = _userManager.GetUserId(User);
            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdentityUserID == userId);
            if (carta.UtilizadorRemetenteFk != utilizador.Id && carta.UtilizadorDestinatarioFk != utilizador.Id)
                return Forbid();

            return View(carta);
        }

        // POST: Cartas/Delete/5
        /// <summary>
        /// Confirma e executa a eliminação de uma carta.
        /// Primeiro, remove as associações na tabela de junção com Categorias e depois remove a carta.
        /// </summary>
        /// <param name="id">O ID da carta a ser eliminada.</param>
        /// <returns>Redireciona para a Index após a eliminação.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carta = await _context.Cartas
                .Include(c => c.Categorias) // Inclui as categorias para poder limpar a relação
                .FirstOrDefaultAsync(m => m.Id == id);

            if (carta == null) return NotFound();
            
            // Limpa as relações na tabela de junção (M-N) antes de apagar a carta
            carta.Categorias.Clear();
            await _context.SaveChangesAsync();

            // Remove a carta da base de dados
            _context.Cartas.Remove(carta);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se uma carta com um determinado ID existe na base de dados.
        /// </summary>
        /// <param name="id">O ID da carta a verificar.</param>
        /// <returns>True se a carta existir, False caso contrário.</returns>
        private bool CartasExists(int id)
        {
            return _context.Cartas.Any(e => e.Id == id);
        }
    }
}