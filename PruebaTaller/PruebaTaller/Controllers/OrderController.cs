using GEJ_Lab.Models;
using GEJ_Lab.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PruebaTaller.Models;
using PruebaTaller.Services;
using System;
using System.Threading.Tasks;

public class OrderController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly PayPalService _payPalService;  // Inyectamos el PayPalService
    private readonly IConfiguration _configuration;
    private readonly CartService _cartService;
    
    private readonly ILogger<OrderController> _logger;

    public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, PayPalService payPalService, IConfiguration configuration, CartService cartService, ILogger<OrderController> logger)
    {
        _context = context;
        _userManager = userManager;
        _payPalService = payPalService; // Inicializamos PayPalService
        _configuration = configuration;
        _cartService = cartService;
        _logger = logger;
    }
    public async Task<IActionResult> CreateAndCompleteOrder(int shippingDetailsId, string paymentId, string payerId)
    {
        _logger.LogInformation("Entrando a CreateAndCompleteOrder con shippingDetailsId: {shippingDetailsId}, paymentId: {paymentId}, payerId: {payerId}", shippingDetailsId, paymentId, payerId);

        try
        {
            // Verificar si los datos de pago fueron proporcionados
            if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(payerId))
            {
                _logger.LogWarning("Datos del pago incompletos.");
                ViewBag.ErrorMessage = "Los datos del pago no fueron proporcionados correctamente.";
                return View("PaymentDataError");
            }

            // Obtener el carrito de compras y el usuario actual
            var cart = _cartService.GetCart();
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogWarning("Usuario no autenticado.");
                ViewBag.ErrorMessage = "Usuario no autenticado. Por favor, inicia sesión.";
                return View("AuthenticationError");
            }

            if (!cart.Items.Any())
            {
                _logger.LogWarning("Carrito vacío.");
                ViewBag.ErrorMessage = "El carrito está vacío. No se puede proceder con el pago.";
                return View("EmptyCartError");
            }

            // Calcular el total de la orden
            var subtotal = cart.CalculateSubtotal();
            var shippingCost = _configuration.GetValue<decimal>("OrderSettings:ShippingCost");
            var totalAmount = subtotal + shippingCost;

            // Obtener token de acceso de PayPal
            var accessToken = await _payPalService.GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogError("No se pudo obtener el token de acceso de PayPal.");
                ViewBag.ErrorMessage = "No se pudo obtener el token de acceso de PayPal. Intenta nuevamente.";
                return View("PaymentError");
            }

            // Verificar si el pago de PayPal ya fue procesado
            if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(payerId))
            {
                // Si no se tiene paymentId o payerId, crear el pago en PayPal
                var returnUrl = $"{Request.Scheme}://{Request.Host}/Order/CreateAndCompleteOrder?shippingDetailsId={shippingDetailsId}";
                var cancelUrl = $"{Request.Scheme}://{Request.Host}/Order/CancelOrder";

                // Crear el pago en PayPal
                var approvalUrl = await _payPalService.CreatePayment(accessToken, totalAmount, returnUrl, cancelUrl);

                _logger.LogInformation("Pago en PayPal creado exitosamente. Redirigiendo a PayPal.");

                // Redirigir al usuario a PayPal para completar el pago
                return Redirect(approvalUrl);
            }
            else
            {
                // Si paymentId y payerId ya están disponibles, completar la orden y registrar el pago

                // Crear la orden en la base de datos
                var order = new Order
                {
                    UserId = user.Id,
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    OrderStatus = "Completed",
                    ShippingMethod = "PayPal",
                    ShippingCost = shippingCost,
                    ShippingDetailsId = shippingDetailsId
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Orden creada exitosamente. OrderId: {OrderId}", order.OrderId);

                // Asociar artículos del carrito con la orden
                foreach (var cartItem in cart.Items)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Price
                    };
                    _context.OrderItems.Add(orderItem);
                }
                await _context.SaveChangesAsync();

                _logger.LogInformation("Artículos del carrito asociados a la orden. OrderId: {OrderId}", order.OrderId);

                // Registrar el pago
                var payment = new Payment
                {
                    OrderId = order.OrderId,
                    UserId = user.Id,
                    Amount = totalAmount,
                    PaymentStatus = "Completed",
                    PaymentMethod = "PayPal",
                    PayPalPaymentId = paymentId,
                    PayPalPayerId = payerId,
                    TransactionDate = DateTime.Now
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Pago registrado exitosamente. PaymentId: {PaymentId}", paymentId);

                // Limpiar el carrito después de completar la orden
                _cartService.ClearCart();

                // Mostrar confirmación de la orden
                ViewBag.OrderId = order.OrderId;
                ViewBag.TotalAmount = order.TotalAmount;
                ViewBag.OrderStatus = order.OrderStatus;

                return View("OrderCompleted");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CreateAndCompleteOrder: {Message}", ex.Message);
            ViewBag.ErrorMessage = $"Error al procesar la orden: {ex.Message}";
            return View("PaymentError");
        }
    }



    // Acción para manejar cancelaciones de pagos
    public IActionResult CancelOrder()
    {
        ViewBag.ErrorMessage = "El pago fue cancelado por el usuario.";
        return View("OrderCancelled");
    }
    public async Task<IActionResult> Payment()
    {
        // Obtener el ClientId de las configuraciones de PayPal
        var clientId = _configuration["PayPalSettings:ClientId"];

        // Verificar si el clientId no está configurado correctamente
        if (string.IsNullOrEmpty(clientId))
        {
            _logger.LogError("El ClientId de PayPal no está configurado correctamente.");
            ViewBag.ErrorMessage = "Error en la configuración de PayPal. Por favor, contacta al soporte.";
            return View("Error");
        }

        // Obtener el usuario actual
        var user = await _userManager.GetUserAsync(User); // Asumiendo que estás usando ASP.NET Identity

        if (user == null)
        {
            _logger.LogError("Usuario no autenticado.");
            ViewBag.ErrorMessage = "Usuario no autenticado. Por favor, inicia sesión.";
            return View("AuthenticationError");
        }

        // Obtener la última orden del usuario, si existe
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.UserId == user.Id && o.OrderStatus == "Pending"); // O el estado que corresponda

        if (order == null)
        {
            _logger.LogError("No se encontró una orden pendiente para el usuario.");
            ViewBag.ErrorMessage = "No se encontró una orden pendiente.";
            return View("EmptyCartError");
        }

        // Obtener los detalles de envío asociados a la orden
        var shippingDetails = await _context.ShippingDetails
            .FirstOrDefaultAsync(sd => sd.OrderId == order.OrderId);

        if (shippingDetails == null)
        {
            _logger.LogError("No se encontraron detalles de envío para esta orden.");
            ViewBag.ErrorMessage = "No se encontraron detalles de envío para esta orden.";
            return View("ShippingError");
        }

        // Calcular el total de la orden
        var totalAmount = order.TotalAmount;

        // Pasar los datos a la vista
        ViewBag.PayPalClientId = clientId;
        ViewBag.TotalAmount = totalAmount.ToString("F2");
        ViewBag.ShippingDetailsId = shippingDetails.ShippingDetailsId;

        return View();
    }





    // Acción para manejar errores en el pago
    public IActionResult PaymentError()
    {
        ViewBag.ErrorMessage = "Error en el pago";
        return View("PaymentError");
    }
}
