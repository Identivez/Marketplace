using Microsoft.AspNetCore.Mvc;
using PruebaTaller.Models;
using System.Diagnostics;
using PruebaTaller.Services;

namespace PruebaTaller.Controllers
{
    /// <summary>
    /// Controlador principal que maneja las vistas de inicio, privacidad y errores.
    /// Muestra los productos m�s recientes y gestiona la informaci�n b�sica del sitio.
    /// </summary>
    public class HomeController : Controller
    {
        // Contexto de la base de datos para interactuar con los productos y otros datos de la aplicaci�n.
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor que inicializa el controlador con el contexto de base de datos.
        /// </summary>
        /// <param name="context">El contexto de base de datos de la aplicaci�n.</param>
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Muestra la vista de inicio con una lista de los productos m�s recientes.
        /// </summary>
        /// <returns>Vista de la p�gina principal con los productos m�s recientes.</returns>
        public IActionResult Index()
        {
            // Obtiene los 4 productos m�s recientes ordenados por su ID de forma descendente.
            var products = _context.Products.OrderByDescending(p => p.Id).Take(6).ToList();
            return View(products);
        }

        /// <summary>
        /// Muestra la vista de privacidad de la aplicaci�n.
        /// </summary>
        /// <returns>Vista de la p�gina de pol�tica de privacidad.</returns>
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ContacUs()
        {
            return View();
        }


        /// <summary>
        /// Muestra la vista de error cuando ocurre una excepci�n o un error no manejado.
        /// </summary>
        /// <returns>Vista de la p�gina de error con los detalles del error.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Crea un modelo de vista de error con el identificador de la solicitud actual.
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
