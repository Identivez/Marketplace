using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaTaller.Models;
using PruebaTaller.Services;

namespace PruebaTaller.Controllers
{
    /// <summary>
    /// Controlador para gestionar las operaciones relacionadas con el carrito de compras.
    /// Permite mostrar, agregar, eliminar y actualizar productos dentro del carrito,
    /// utilizando el almacenamiento de sesión para gestionar el estado del carrito.
    /// </summary>
    public class CartController : Controller
    {
        // Contexto de base de datos para interactuar con los productos
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor que inicializa el controlador con el contexto de base de datos.
        /// </summary>
        /// <param name="context">El contexto de la base de datos de la aplicación.</param>
        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Muestra la vista del carrito de compras con los productos actuales.
        /// </summary>
        /// <returns>Vista con los elementos del carrito.</returns>
        public IActionResult Index()
        {
            // Obtener el carrito de la sesión o crear uno nuevo si no existe
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            ViewBag.CartItemCount = cart.Items.Sum(item => item.Quantity);
            return View(cart.GetItems());
        }

        public IActionResult Checkout()
        {
            var model = new List<Cart_Item>();
            return View(model);
        }

        public IActionResult Payment()
        {
            return View();
        }

        /// <summary>
        /// Agrega un producto al carrito de compras.
        /// </summary>
        /// <param name="id">El identificador del producto a agregar.</param>
        /// <returns>Redirige a la vista del carrito.</returns>
        public IActionResult AddToCart(int id)
        {
            // Buscar el producto por su ID en la base de datos
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            // Obtener el carrito de la sesión o crear uno nuevo si no existe
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            // Crear un nuevo ítem de carrito con el producto seleccionado
            var cartItem = new Cart_Item
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                ImageFileName = product.ImageFileName,
                Quantity = 1
            };

            // Agregar el ítem al carrito
            cart.AddItem(cartItem);

            // Guardar el carrito actualizado en la sesión
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Actualizar la cantidad de elementos en ViewBag
            ViewBag.CartItemCount = cart.Items.Sum(item => item.Quantity);

            // Redirigir a la vista del carrito
            return RedirectToAction("Index", "Cart");
        }

        /// <summary>
        /// Elimina un producto del carrito de compras.
        /// </summary>
        /// <param name="id">El identificador del producto a eliminar.</param>
        /// <returns>Redirige a la vista del carrito actualizado.</returns>
        [HttpPost]
        public IActionResult RemoveItem(int id)
        {
            // Obtener el carrito de la sesión o crear uno nuevo si no existe
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            // Eliminar el ítem del carrito por su ID
            cart.RemoveItem(id);

            // Guardar el carrito actualizado en la sesión
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Actualizar la cantidad de elementos en ViewBag
            ViewBag.CartItemCount = cart.Items.Sum(item => item.Quantity);

            // Redirigir a la vista del carrito
            return Json(new { success = true });
        }

        /// <summary>
        /// Elimina todos los productos del carrito de compras.
        /// </summary>
        /// <returns>Redirige a la vista del carrito actualizado.</returns>
        [HttpPost]
        public IActionResult ClearCart()
        {
            // Crear un nuevo carrito vacío
            var cart = new ShoppingCart();

            // Guardar el carrito vacío en la sesión
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Actualizar la cantidad de elementos en ViewBag
            ViewBag.CartItemCount = 0;

            // Redirigir a la vista del carrito
            return Json(new { success = true });
        }

        /// <summary>
        /// Actualiza las cantidades de los productos en el carrito de compras.
        /// </summary>
        /// <param name="quantities">Diccionario que contiene los IDs de los productos y las cantidades actualizadas.</param>
        /// <returns>Redirige a la vista del carrito actualizado.</returns>
        [HttpPost]
        public IActionResult UpdateCart(Dictionary<int, int> quantities)
        {
            // Obtener el carrito de la sesión o crear uno nuevo si no existe
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            // Actualizar la cantidad de cada producto en el carrito
            foreach (var entry in quantities)
            {
                cart.UpdateItemQuantity(entry.Key, entry.Value);
            }

            // Guardar el carrito actualizado en la sesión
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Redirigir al carrito para ver los cambios
            return Json(new { success = true });
        }
    }
}
