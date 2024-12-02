//using GEJ_Lab.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using PruebaTaller.Models;
//using PruebaTaller.Services;
//using System;
//using System.Threading.Tasks;

//public class OrderController : Controller
//{
//    private readonly UserManager<ApplicationUser> _userManager;
//    private readonly ApplicationDbContext _context;
//    private readonly PayPalService _payPalService;  // Inyectamos el PayPalService
//    private readonly IConfiguration _configuration;

//    public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, PayPalService payPalService, IConfiguration configuration)
//    {
//        _context = context;
//        _userManager = userManager;
//        _payPalService = payPalService; // Inicializamos PayPalService
//        _configuration = configuration;
//    }

//    // Acción para crear la orden y redirigir al usuario a PayPal
//    public async Task<IActionResult> CreateOrder(decimal totalAmount)
//    {
//        var user = await _userManager.GetUserAsync(User);

//        // Genera dinámicamente las URLs de retorno y cancelación
//        var returnUrl = $"{Request.Scheme}://{Request.Host}/order/complete";
//        var cancelUrl = $"{Request.Scheme}://{Request.Host}/order/cancel";

//        try
//        {
//            // Obtener las configuraciones de PayPal desde el archivo de configuración
//            var clientId = _configuration["PayPalSettings:ClientId"];
//            var secret = _configuration["PayPalSettings:Secret"];
//            var environment = _configuration["PayPalSettings:Environment"];

//            // Validar que las configuraciones no sean nulas o vacías
//            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(environment))
//            {
//                ViewBag.ErrorMessage = "Las configuraciones de PayPal no están completas. Por favor, verifica las credenciales.";
//                return View("PaymentError");  // Muestra la vista de error si las configuraciones están incompletas
//            }

//            // Obtener el token de acceso de PayPal
//            var accessToken = await _payPalService.GetAccessToken();
//            if (string.IsNullOrEmpty(accessToken))
//            {
//                ViewBag.ErrorMessage = "No se pudo obtener el token de acceso de PayPal. Intenta nuevamente.";
//                return View("PaymentError");
//            }

//            // Crear el pago con PayPal
//            var approvalUrl = await _payPalService.CreatePayment(accessToken, totalAmount, returnUrl, cancelUrl);

//            // Crear una nueva orden en la base de datos
//            var order = new Order
//            {
//                UserId = user.Id,
//                OrderDate = DateTime.Now,
//                TotalAmount = totalAmount,
//                OrderStatus = "Pending", // Estado pendiente antes de la confirmación del pago
//                ShippingAddress = "Dirección de envío", // Ajusta esto según tu modelo
//                ShippingMethod = "Método de envío", // Ajusta según tu modelo
//                ShippingCost = 5.00m // Ajusta según tus necesidades
//            };

//            _context.Orders.Add(order);
//            await _context.SaveChangesAsync(); // Guardamos la orden

//            if (order.OrderId > 0)  // Verifica que la orden fue guardada correctamente
//            {
//                // Crear un pago y asociarlo a la orden
//                var payment = new Payment
//                {
//                    OrderId = order.OrderId,
//                    UserId = user.Id,
//                    Amount = totalAmount,
//                    PaymentStatus = "Pending",
//                    PaymentMethod = "PayPal",
//                    TransactionDate = DateTime.Now
//                };

//                _context.Payments.Add(payment);
//                await _context.SaveChangesAsync();  // Guardamos el pago

//                if (payment.PaymentId > 0)  // Verifica que el pago fue guardado correctamente
//                {
//                    // Redirigir al usuario a PayPal para que complete el pago
//                    ViewBag.SuccessMessage = "La orden ha sido creada correctamente. Ahora será redirigido a PayPal.";
//                    return Redirect(approvalUrl);  // Redirigimos a PayPal
//                }
//                else
//                {
//                    ViewBag.ErrorMessage = "Hubo un problema al crear el pago en el sistema.";
//                    return View("PaymentError");  // Vuelve a la vista si el pago no se guardó correctamente
//                }
//            }
//            else
//            {
//                ViewBag.ErrorMessage = "Hubo un problema al crear la orden en el sistema.";
//                return View("OrderCreationFailed");  // Muestra la vista si la orden no se creó correctamente
//            }
//        }
//        catch (Exception ex)
//        {
//            ViewBag.ErrorMessage = $"Error al crear la orden: {ex.Message}";
//            return View("OrderCreationError");  // Vuelve a la vista en caso de error al crear la orden
//        }
//    }

//    // Acción para completar la orden después de la autorización de PayPal
//    [HttpPost]
//    public async Task<IActionResult> CompleteOrder([FromBody] PaymentData paymentData)
//    {
//        if (paymentData == null || string.IsNullOrEmpty(paymentData.paymentId) || string.IsNullOrEmpty(paymentData.payerId) || string.IsNullOrEmpty(paymentData.orderId))
//        {
//            ViewBag.ErrorMessage = "Los datos del pago no fueron proporcionados correctamente o están incompletos.";
//            return View("PaymentDataError");  // Muestra la vista con el mensaje de error
//        }

//        var paymentId = paymentData.paymentId;
//        var payerId = paymentData.payerId;
//        var orderId = paymentData.orderId;

//        try
//        {
//            // Verificar si la orden ya existe
//            var order = await _context.Orders
//                .FirstOrDefaultAsync(o => o.OrderId == int.Parse(orderId)); // Convierte el orderId a int si es necesario

//            // Si la orden no existe, la creamos
//            if (order == null)
//            {
//                order = new Order
//                {
//                    UserId = payerId,  // Suponemos que payerId es el ID del usuario (ajustalo según el modelo)
//                    OrderDate = DateTime.Now,
//                    TotalAmount = ViewBag.TotalAmount,  // Esto puede ser actualizado más tarde con el total de la transacción
//                    OrderStatus = "Pending",  // Estado inicial
//                    ShippingAddress = "Dirección no especificada",  // Ajusta según el flujo de tu aplicación
//                    ShippingMethod = "Paypal",  // Ajusta según el flujo de tu aplicación
//                    ShippingCost = 0 // Ajusta según el flujo de tu aplicación
//                };

//                _context.Orders.Add(order);
//                var saveResult = await _context.SaveChangesAsync(); // Guardamos la nueva orden

//                // Verificación de si la orden fue guardada correctamente
//                if (saveResult <= 0)
//                {
//                    ViewBag.ErrorMessage = "Hubo un problema al guardar la orden en la base de datos.";
//                    return View("OrderCreationFailed");
//                }

//                // Información adicional para depuración
//                Console.WriteLine("Orden creada correctamente. ID: " + order.OrderId);
//            }

//            // Crear un nuevo pago para la orden (si no existe ya)
//            var payment = await _context.Payments
//                .FirstOrDefaultAsync(p => p.PayPalPaymentId == paymentId);

//            if (payment == null)
//            {
//                payment = new Payment
//                {
//                    OrderId = order.OrderId,
//                    UserId = order.UserId,
//                    Amount = order.TotalAmount, // Asegúrate de actualizar esto con el monto real del pago
//                    PaymentStatus = "Pending",
//                    PaymentMethod = "PayPal",
//                    PayPalPaymentId = paymentId,
//                    PayPalPayerId = payerId,
//                    TransactionDate = DateTime.Now
//                };

//                _context.Payments.Add(payment);  // Guardamos el nuevo pago
//                var paymentSaveResult = await _context.SaveChangesAsync();

//                // Verificación de si el pago fue guardado correctamente
//                if (paymentSaveResult <= 0)
//                {
//                    ViewBag.ErrorMessage = "Hubo un problema al guardar el pago en la base de datos.";
//                    return View("PaymentError");
//                }

//                // Información adicional para depuración
//                Console.WriteLine("Pago creado correctamente. ID: " + payment.PaymentId);
//            }
//            else
//            {
//                // Si el pago ya existe, lo actualizamos
//                payment.PayPalPayerId = payerId;
//                payment.PaymentStatus = "Completed";  // Actualizamos el estado del pago
//                payment.TransactionDate = DateTime.Now;

//                _context.Update(payment);
//                await _context.SaveChangesAsync();
//            }

//            // Actualizar la orden con el estado "Completed"
//            order.OrderStatus = "Completed";  // Cambiamos el estado de la orden
//            _context.Update(order);
//            await _context.SaveChangesAsync();  // Guardamos la actualización de la orden

//            // Información adicional para depuración
//            Console.WriteLine("Orden actualizada. Nuevo estado: " + order.OrderStatus);

//            // Pasar datos de la orden a la vista
//            ViewBag.OrderId = order.OrderId;
//            ViewBag.TotalAmount = order.TotalAmount;
//            ViewBag.OrderStatus = order.OrderStatus;
//            ViewBag.PaymentStatus = payment.PaymentStatus;

//            return View("OrderCompleted");  // Redirige a la vista de confirmación de la orden
//        }
//        catch (Exception ex)
//        {
//            ViewBag.ErrorMessage = $"Error al procesar el pago: {ex.Message}";
//            return View("PaymentError");  // Muestra la vista con el mensaje de error
//        }
//    }



//    // Acción para manejar cancelaciones de pagos
//    public IActionResult CancelOrder()
//    {
//        ViewBag.ErrorMessage = "El pago fue cancelado por el usuario.";
//        return View("OrderCancelled");
//    }

//    // Acción para mostrar el Client ID de PayPal en la vista
//    public async Task<IActionResult> Payment()
//    {
//        // Obtener el ClientId de las configuraciones
//        var clientId = _configuration["PayPalSettings:ClientId"];

//        // Pasarlo a la vista usando ViewBag
//        ViewBag.PayPalClientId = clientId;

//        return View();
//    }

//    // Acción para manejar errores en el pago
//    public IActionResult PaymentError()
//    {
//        ViewBag.ErrorMessage = "Error en el pago";
//        return View("PaymentError");
//    }
//}
