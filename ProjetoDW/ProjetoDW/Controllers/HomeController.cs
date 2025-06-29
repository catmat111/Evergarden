using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoDW.Models;

namespace ProjetoDW.Controllers
{
    /// <summary>
    /// Controlador principal da aplicação, responsável por gerir as páginas iniciais,
    /// como a Home, a página de Privacidade e a página de Erro.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="HomeController"/>.
        /// </summary>
        /// <param name="logger">O serviço de logging para registar informações e erros.</param>
        /// <param name="userManager">O serviço de gestão de utilizadores do Identity.</param>
        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        /// Apresenta a página inicial (Home) da aplicação.
        /// </summary>
        /// <returns>A View correspondente à página principal.</returns>
        public async Task<IActionResult> Index()
        {
            // Obtém o utilizador atualmente autenticado.
            var currentUser = await _userManager.GetUserAsync(User);
            
            // Se existir um utilizador autenticado, pode-se obter os seus papéis (roles).
            // Esta informação não está a ser usada atualmente, mas está disponível se for necessário.
            if (currentUser != null)
            {
                var roles = await _userManager.GetRolesAsync(currentUser);
                // A variável 'i' não tem utilização prática no código atual.
                var i = "";
            }
            
            // Retorna a view principal "Index".
            return View();
        }

        /// <summary>
        /// Apresenta a página de Política de Privacidade.
        /// </summary>
        /// <returns>A View "Privacy".</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Apresenta uma página de erro genérica.
        /// O ID do pedido é obtido para fins de rastreamento e depuração.
        /// </summary>
        /// <returns>A View de Erro com um modelo <see cref="ErrorViewModel"/>.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Cria um modelo de erro e preenche o RequestId com o ID da atividade atual
            // ou com o identificador de rastreamento do HttpContext.
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}