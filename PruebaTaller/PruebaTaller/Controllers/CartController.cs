using GEJ_Lab.Models;
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
       
        private readonly IConfiguration configuration;
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

        //public IActionResult Checkout()
        //{
        //    var model = new List<Cart_Item>();
        //    return View(model);
        //}
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCartFromSession();  // Asumimos que esta función obtiene el carrito de la sesión

            if (!cart.Items.Any())  // Verificamos si el carrito está vacío
            {
                TempData["ErrorMessage"] = "El carrito está vacío. Agrega productos antes de proceder al pago.";
                return RedirectToAction("Index");
            }

            // Calcular el subtotal, costo de envío y total
            var subtotal = cart.Items.Sum(item => item.Price * item.Quantity);
            var shippingCost = 20.00m;  // Costo fijo de envío (puede ser calculado dinámicamente si es necesario)
            var totalAmount = subtotal + shippingCost;
            // Total general (subtotal + envío)

            ViewBag.TotalAmount = totalAmount;

            // Crear el DTO para pasar la información a la vista
            var shippingDetailsDTO = new ShippingDetailsDTO
            {
                Items = cart.GetItems(),
                Subtotal = subtotal,
                ShippingCost = shippingCost,
                TotalAmount = totalAmount  // Total final con envío
            };

            // Pasamos el total a ViewBag para usarlo en JavaScript para el botón de PayPal


            return View(shippingDetailsDTO);
        }
        private ShoppingCart GetCartFromSession()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null)
            {
                cart = new ShoppingCart();
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return cart;
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
        // Acción para mostrar la vista de formulario de envío
        [HttpGet]
        public IActionResult ShippingForm()
        {
            var shippingDetailsDto = new ShippingDetailsDTOO();
            return View(shippingDetailsDto);
        }

        // Acción para guardar los detalles de envío
        [HttpPost]
        public async Task<IActionResult> CreateShippingDetails(ShippingDetailsDTOO shippingDetailsDto)
        {
            // Valida el modelo sin considerar OrderId
            if (!ModelState.IsValid)
            {
                // Muestra los errores específicos en la vista.
                ViewBag.ErrorMessage = "Invalid data provided. Please check the fields below.";
                return View("ShippingForm", shippingDetailsDto);
            }

            try
            {
                // Mapear DTO a la entidad
                var shippingDetails = new ShippingDetails
                {
                    // OrderId no se asigna aquí
                    FullName = shippingDetailsDto.FullName,
                    Address = shippingDetailsDto.Address,
                    City = shippingDetailsDto.City,
                    State = shippingDetailsDto.State,
                    ZipCode = shippingDetailsDto.ZipCode,
                    PhoneNumber = shippingDetailsDto.PhoneNumber
                };

                // Guardar en la base de datos
                _context.ShippingDetails.Add(shippingDetails);
                await _context.SaveChangesAsync();

                // Redirigir a la confirmación
                return RedirectToAction("ShippingDetailsConfirmation");
            }
            catch (DbUpdateException ex)
            {
                // Manejo de errores específicos de la base de datos
                ViewBag.ErrorMessage = $"Database error: {ex.Message}";
                return View("ShippingError");
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                ViewBag.ErrorMessage = $"Unexpected error: {ex.Message}";
                return View("ShippingError");
            }
        }


        // Acción para confirmar que los detalles de envío se guardaron correctamente
        public IActionResult ShippingDetailsConfirmation()
        {
            ViewBag.SuccessMessage = "Shipping details have been saved successfully.";
            return View();
        }

        // Acción para manejar errores al guardar los detalles de envío
        public IActionResult ShippingError()
        {
            ViewBag.ErrorMessage = "An error occurred while saving the shipping details. Please try again.";
            return View();
        }
    }
   
}

