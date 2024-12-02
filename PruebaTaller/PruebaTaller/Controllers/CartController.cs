using GEJ_Lab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaTaller.Models;
using PruebaTaller.Services;

namespace PruebaTaller.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public CartController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Muestra la vista del carrito con los productos actuales
        public IActionResult Index()
        {
            var cart = GetCartFromSession();
            ViewBag.CartItemCount = cart.Items.Sum(item => item.Quantity);
            return View(cart.GetItems());
        }
        [HttpGet]
        public IActionResult Checkout()
        {
            try
            {
                var cart = GetCartFromSession();

                if (!cart.Items.Any())
                {
                    TempData["ErrorMessage"] = "El carrito está vacío. Agrega productos antes de proceder al pago.";
                    return RedirectToAction("Index");
                }

                // Calcular el subtotal, costo de envío y total
                var subtotal = cart.CalculateSubtotal();
                var shippingCost = _configuration.GetValue<decimal>("OrderSettings:ShippingCost");
                if (shippingCost <= 0)
                {
                    TempData["ErrorMessage"] = "El costo de envío no está configurado correctamente.";
                    return RedirectToAction("Index");
                }

                var totalAmount = subtotal + shippingCost;

                // Pasar el total al ViewBag para la vista
                ViewBag.TotalAmount = totalAmount;

                // Crear el DTO con los detalles del carrito y costos calculados
                var shippingDetailsDTO = new ShippingDetailsDTO
                {
                    Items = cart.GetItems(),
                    Subtotal = subtotal,
                    ShippingCost = shippingCost,
                    TotalAmount = totalAmount
                };

                // Renderizar la vista de Checkout con los datos calculados
                return View(shippingDetailsDTO);
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                TempData["ErrorMessage"] = $"Ocurrió un error al preparar la página de pago: {ex.Message}";
                return RedirectToAction("Index");
            }
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

        // Agrega un producto al carrito
        public IActionResult AddToCart(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            var cart = GetCartFromSession();
            cart.AddItem(new Cart_Item
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                ImageFileName = product.ImageFileName,
                Quantity = 1
            });

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            ViewBag.CartItemCount = cart.Items.Sum(item => item.Quantity);
            return RedirectToAction("Index", "Cart");
        }

        // Elimina un producto del carrito
        [HttpPost]
        public IActionResult RemoveItem(int id)
        {
            var cart = GetCartFromSession();
            cart.RemoveItem(id);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            ViewBag.CartItemCount = cart.Items.Sum(item => item.Quantity);
            return Json(new { success = true });
        }

        // Vacía el carrito
        [HttpPost]
        public IActionResult ClearCart()
        {
            var cart = new ShoppingCart();
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            ViewBag.CartItemCount = 0;
            return Json(new { success = true });
        }

        // Actualiza las cantidades de los productos en el carrito
        [HttpPost]
        public IActionResult UpdateCart(Dictionary<int, int> quantities)
        {
            var cart = GetCartFromSession();
            foreach (var entry in quantities)
            {
                cart.UpdateItemQuantity(entry.Key, entry.Value);
            }
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return Json(new { success = true });
        }

        // Muestra el formulario para los detalles de envío
        [HttpGet]
        public IActionResult ShippingForm()
        {
            var shippingDetailsDto = new ShippingDetailsDTOO();
            return View(shippingDetailsDto);
        }

        // Guarda los detalles de envío
        [HttpPost]
        public async Task<IActionResult> CreateShippingDetails(ShippingDetailsDTOO shippingDetailsDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Invalid data provided. Please check the fields below.";
                return View("ShippingForm", shippingDetailsDto);
            }

            try
            {
                var shippingDetails = new ShippingDetails
                {
                    FullName = shippingDetailsDto.FullName,
                    Address = shippingDetailsDto.Address,
                    City = shippingDetailsDto.City,
                    State = shippingDetailsDto.State,
                    ZipCode = shippingDetailsDto.ZipCode,
                    PhoneNumber = shippingDetailsDto.PhoneNumber
                };

                _context.ShippingDetails.Add(shippingDetails);
                await _context.SaveChangesAsync();

                TempData["ShippingDetailsId"] = shippingDetails.ShippingDetailsId;

                return RedirectToAction("ShippingDetailsConfirmation");
            }
            catch (DbUpdateException ex)
            {
                ViewBag.ErrorMessage = $"Database error: {ex.Message}";
                return View("ShippingError");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Unexpected error: {ex.Message}";
                return View("ShippingError");
            }
        }

        public IActionResult ShippingDetailsConfirmation()
        {
            ViewBag.SuccessMessage = "Shipping details have been saved successfully.";
            return View();
        }

        public IActionResult ShippingError()
        {
            ViewBag.ErrorMessage = "An error occurred while saving the shipping details. Please try again.";
            return View();
        }

        public async Task<IActionResult> Payment()
        {
            return View("Payment");
        }

        // Limpia el carrito después de la creación de la orden
        private void ClearCartAfter()
        {
            HttpContext.Session.Remove("Cart");
        }
    }
}
