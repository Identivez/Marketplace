using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GEJ_Lab.Migrations;
using PruebaTaller.Services;
using System.Linq;
using PruebaTaller.Models;
using System.Drawing.Printing;

namespace GEJ_Lab.Controllers
{
    /// <summary>
    /// Controlador para gestionar las operaciones relacionadas con la tienda.
    /// Permite mostrar una lista de productos y visualizar los detalles de cada producto.
    /// </summary>
    public class StoreController : Controller
    {
        // Contexto de la base de datos para interactuar con los productos.
        private readonly ApplicationDbContext _context;
        private readonly int pageSize = 5; // Tamaño de página para la paginación

        /// <summary>
        /// Constructor que inicializa el controlador con el contexto de base de datos.
        /// </summary>
        /// <param name="context">El contexto de base de datos de la aplicación.</param>
        public StoreController(ApplicationDbContext context)
        {
            this._context = context;
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

            // Crear el ViewModel para los detalles del producto
            var viewModel = new ProductDetailsView
            {
                Product = product,
                RelatedProducts = relatedProducts
            };

            // Pasar el ViewModel a la vista
            return View(viewModel);
        }

        /// <summary>
        /// Muestra la vista principal de la tienda con una lista de productos, basada en filtros y paginación.
        /// </summary>
        /// <param name="search">Texto de búsqueda para filtrar productos por nombre.</param>
        /// <param name="selectedBrand">Marca seleccionada para filtrar productos.</param>
        /// <param name="selectedCategory">Categoría seleccionada para filtrar productos.</param>
        /// <param name="sort">Opción de ordenamiento seleccionada por el usuario.</param>
        /// <param name="pageIndex">Índice de la página actual para la paginación.</param>
        /// <returns>Vista con los productos filtrados y paginados.</returns>
        public IActionResult Index(string? search, string? selectedBrand, string? selectedCategory, string? sort, int pageIndex = 1)
        {
            int pageSize =6; // Número de productos por página
            IQueryable<Product> query = _context.Products;

            // Filtrado por nombre (búsqueda)
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }

            // Filtrado por marca
            if (!string.IsNullOrEmpty(selectedBrand))
            {
                query = query.Where(p => p.Brand == selectedBrand);
            }

            // Filtrado por categoría
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                query = query.Where(p => p.Category == selectedCategory);
            }

            // Ordenación de productos según el criterio seleccionado
            switch (sort)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case "newest":
                default:
                    query = query.OrderByDescending(p => p.Id);
                    break;
            }

            // Total de productos después de aplicar filtros
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Paginación
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            var products = query.ToList();

            // Obtener las marcas y categorías disponibles para los filtros
            var brands = _context.Products.Select(p => p.Brand).Distinct().ToList();
            var categories = _context.Products.Select(p => p.Category).Distinct().ToList();

            // Crear el ViewModel para la búsqueda en la tienda
            var storeSearchModel = new StoreSearchModel()
            {
                Search = search,
                SelectedBrand = selectedBrand,
                SelectedCategory = selectedCategory,
                Sort = sort,
                Brands = brands,
                Categories = categories,
                Products = products // Añadir la lista de productos directamente al modelo
            };

            // Pasar datos adicionales para la paginación a la vista mediante ViewData
            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;

            return View(storeSearchModel);
        }
    }
}
