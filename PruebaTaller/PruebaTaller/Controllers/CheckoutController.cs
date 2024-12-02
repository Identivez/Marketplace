using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using System.Text;

namespace GEJ_Lab.Controllers
{
    public class CheckoutController : Controller
    {
        private string _PaypalClientId { get; set; } = "";
        private string _PaypalSecret { get; set; } = "";
        private string _PaypalUrl { get; set; } = "";

        public CheckoutController(IConfiguration configuration)
        {

            //encabezado
            _PaypalClientId = configuration["PaypalSettings:ClientId"]!;//estructura completa 
            // accede al archivo de la configuracion, mientras no le de un token de PayPal
            _PaypalSecret = configuration["PaypalSettings:Secret"]!;
            _PaypalUrl = configuration["PaypalSettings:Url"]!;

        }

        private async Task<string> GetPaypalAccessToken()
        {
            string accessToken = "";//variable cadena inicializada de manera vacia

            //viene del archivo de configuracion
            string url = _PaypalUrl + "/v1/oauth2/token";//url que viene del post//  liga completa que llama al TOKEN
            //Servidor=  _PaypalUrl

            using (var client = new HttpClient())//HttpClient trabajar con un post y va a hacer la peticion
            {
                //UTF8 Va decodificar correctamente
                string credentials64 =
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(_PaypalClientId + ":" + _PaypalSecret));//unimos y concatenamos el client id    -convierte las credenciales 

                //En el encabezado "Authorization", el valor "Basic " indica el tipo de autenticación, seguido de las credenciales codificadas en 
                //Base64. 
                //Authorization: Basic dXNlcm5hbWU6cGFzc3dvcmQ= El encabezado en texto plano se veria asi. 
                //Headers encabezado 
                //Checar pagina de paypal lo pide 
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials64);


                //Crear una solicitud HTTP POST con HttpRequestMessage y
                //establecer el contenido en "grant_type=client_credentials".
                //Esto se usa típicamente en el flujo de autenticación de OAuth 2.0,
                //donde el cliente solicita un token de acceso para acceder a recursos protegidos.


                //es importante establecer el encabezado Content-Type en "application/x-www-form-urlencoded"
                //ya que el cuerpo de la solicitud está en formato URL codificada.


                //HttpRequestMessage la solicitud del mensaje,la peticion ya armada 

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("grant_type=client_credentials", null
                    , "application/x-www-form-urlencoded");

                var httpResponse = await client.SendAsync(requestMessage);

                //Esperamos la siguiente respuesta

                //                {
                //                    "scope": "https://uri.paypal.com/services/invoicing https://uri.paypal.com/services/disputes/read-buyer https://uri.paypal.com/services/payments/realtimepayment https://uri.paypal.com/services/disputes/update-seller https://uri.paypal.com/services/payments/payment/authcapture openid https://uri.paypal.com/services/disputes/read-seller https://uri.paypal.com/services/payments/refund https://api-m.paypal.com/v1/vault/credit-card https://api-m.paypal.com/v1/payments/.* https://uri.paypal.com/payments/payouts https://api-m.paypal.com/v1/vault/credit-card/.* https://uri.paypal.com/services/subscriptions https://uri.paypal.com/services/applications/webhooks",
                //                    "access_token": "A21AAFEpH4PsADK7qSS7pSRsgzfENtu-Q1ysgEDVDESseMHBYXVJYE8ovjj68elIDy8nF26AwPhfXTIeWAZHSLIsQkSYz9ifg",
                //                    "token_type": "Bearer",
                //                    "app_id": "APP-80W284485P519543T",
                //                    "expires_in": 31668,
                //                    "nonce": "2020-04-03T15:35:36ZaYZlGvEkV4yVSz8g6bAKFoGSEzuy3CQcz3ljhibkOHg"
                //                  }

                //Success propiedad de httpResponse 
                if (httpResponse.IsSuccessStatusCode)//httpResponse el token si todo salio bien
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();//Primero lo leemos como una variable

                    var jsonResponse = JsonNode.Parse(strResponse);//(convertimos esa cadena en  jason porque la respuesta viene en cadena ,no sabe
                                                                   //que es un jason pero viene listo para ser un jason )porque viene en ese formato
                    if (jsonResponse != null)
                    {
                        accessToken = jsonResponse["access_token"]?.ToString() ?? "";
                    }
                }
                else
                    accessToken = "No autorizado";
            }


            return accessToken;
        }

        public async Task<string> Token()
        {
            return await GetPaypalAccessToken();
        }
        public IActionResult Index()
        {
            ViewBag.PaypalClientId = _PaypalClientId;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CreateOrder([FromBody] JsonObject data)
        {
            var totalAmount = data?["amount"]?.ToString();

            //Si el monto es vacio returnamos el Id de orden vacio 
            if (totalAmount == null)
            {
                return new JsonResult(new { Id = "" });
            }
            //Si el monto es correcto, hacemos una petición al servidor de PayPal


            // Creamos el request body
            JsonObject createOrderRequest = new JsonObject();
            createOrderRequest.Add("intent", "CAPTURE");

            JsonObject amount = new JsonObject();
            amount.Add("currency_code", "USD");
            amount.Add("value", totalAmount);

            JsonObject purchaseUnit1 = new JsonObject();
            purchaseUnit1.Add("amount", amount);

            JsonArray purchaseUnits = new JsonArray();
            purchaseUnits.Add(purchaseUnit1);

            createOrderRequest.Add("purchase_units", purchaseUnits);

            //Obtenemos el token 
            string accessToken = await GetPaypalAccessToken();

            //URL Request
            string url = _PaypalUrl + "/v2/checkout/orders"; //URL de la API de PayPal, esta en la documentación

            //Con el objeto HttpClient hacemos la petición
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent(createOrderRequest.ToString(), null, "application/json");

                var httpResponse = await client.SendAsync(requestMessage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponse = JsonNode.Parse(strResponse);

                    if (jsonResponse != null)
                    {
                        string paypalOrderId = jsonResponse["id"]?.ToString() ?? "";

                        return new JsonResult(new { Id = paypalOrderId });
                    }
                }
            }

            return new JsonResult(new { Id = "" });
        }

        [HttpPost]
        public async Task<JsonResult> CompleteOrder([FromBody] JsonObject data)
        {
            var orderId = data?["orderID"]?.ToString();

            if (orderId == null)
            {
                return new JsonResult("error");
            }
            // get access token
            string accessToken = await GetPaypalAccessToken();
            string url = _PaypalUrl + "/v2/checkout/orders/" + orderId + "/capture";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("", null, "application/json");

                var httpResponse = await client.SendAsync(requestMessage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponse = JsonNode.Parse(strResponse);

                    if (jsonResponse != null)
                    {
                        string paypalOrderStatus = jsonResponse["status"]?.ToString() ?? "";
                        if (paypalOrderStatus == "COMPLETED")
                        {
                            // save the order in the database
                            //await SaveOrder(jsonResponse.ToString(), deliveryAddress);

                            return new JsonResult("success");
                        }
                    }
                }
            }
            return new JsonResult("error");


        }
    }
}
