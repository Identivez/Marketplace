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
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            ViewBag.CartItemCount = cart.Items.Sum(item => item.Quantity);
            return View(cart.GetItems());
        }

        public IActionResult AddToCart(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var cartItem = new Cart_Item
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageFileName = product.ImageFileName,
                Quantity = 1
            };

            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            ViewBag.CartItemCount = cart.Items.Sum(item => item.Quantity); // Actualizar el valor del ViewBag

            return RedirectToAction("Index", "Cart");
        }

        public IActionResult RemoveItem(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.RemoveItem(id);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            ViewBag.CartItemCount = cart.Items.Sum(item => item.Quantity); // Actualizar el valor del ViewBag

            return RedirectToAction("Index");
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
            return RedirectToAction("Index");
        }
    }
}
