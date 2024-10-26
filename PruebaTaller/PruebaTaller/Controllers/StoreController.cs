using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaTaller.Services;
using PruebaTaller.Models;
using System.Linq;
namespace PruebaTaller.Controllers
{
    /// <summary>
    /// Controlador para gestionar las operaciones relacionadas con la tienda.
    /// Permite mostrar una lista de productos y visualizar los detalles de cada producto.
    /// </summary>
    public class StoreController : Controller
    {
        // Contexto de la base de datos para interactuar con los productos.
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor que inicializa el controlador con el contexto de base de datos.
        /// </summary>
        /// <param name="context">El contexto de base de datos de la aplicación.</param>
        public StoreController(ApplicationDbContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// Muestra la vista principal de la tienda con la lista de productos.
        /// </summary>
        /// <returns>Vista con una lista de todos los productos disponibles.</returns>
        public IActionResult Index()
        {
            // Obtener todos los productos de la base de datos.
            var products = _context.Products.ToList();
            return View(products);
        }

        /// <summary>
        /// Muestra los detalles de un producto específico.
        /// </summary>
        /// <param name="id">ID del producto a visualizar.</param>
        /// <returns>Vista con los detalles del producto, o redirige a la página principal si no se encuentra.</returns>
        [HttpGet]
        public IActionResult Details(int id)
        {
            // Buscar el producto por su ID
            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Obtener productos relacionados de la misma categoría (excluyendo el producto actual)
            var relatedProducts = _context.Products
                .Where(p => p.Category == product.Category && p.Id != id)
                .Take(4)
                .ToList();

            // Crear el ViewModel
            var viewModel = new ProductDetailsView
            {
                Product = product,
                RelatedProducts = relatedProducts
            };

            // Pasar el ViewModel a la vista
            return View(viewModel);
        }
    }
}
