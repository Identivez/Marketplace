using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GEJ_Lab.Models
{
    public class PayPalService
    {
        private readonly IConfiguration _configuration;
        private readonly string _paypalUrl;

        // Constructor para inyectar IConfiguration
        public PayPalService(IConfiguration configuration)
        {
            _configuration = configuration;
            _paypalUrl = _configuration["PayPalSettings:PaypalUrl"];
        }

        // Obtener el token de acceso de PayPal
        public async Task<string> GetAccessToken()
        {
            try
            {
                string clientId = _configuration["PayPalSettings:ClientId"];
                string secret = _configuration["PayPalSettings:Secret"];
                string environment = _configuration["PayPalSettings:Environment"];

                // Selecciona la URL según el entorno (sandbox o producción)
                var tokenUrl = environment == "sandbox" ?
                    "https://api.sandbox.paypal.com/v1/oauth2/token" :
                    "https://api.paypal.com/v1/oauth2/token";

                using (var client = new HttpClient())
                {
                    // Usar credenciales básicas (Basic Auth)
                    var byteArray = Encoding.ASCII.GetBytes($"{clientId}:{secret}");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    // Crear el cuerpo de la solicitud POST
                    var requestBody = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "client_credentials")
                    });

                    // Hacer la solicitud POST a la URL de token
                    var response = await client.PostAsync(tokenUrl, requestBody);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Error al obtener el token de PayPal: " + response.ReasonPhrase);
                    }

                    // Leer la respuesta y obtener el token de acceso
                    var responseString = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);

                    // Verificar si la respuesta contiene el token
                    if (jsonResponse?.access_token != null)
                    {
                        return jsonResponse.access_token; // Devolver el token de acceso
                    }
                    else
                    {
                        throw new Exception("No se obtuvo el token de PayPal.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el token de PayPal: {ex.Message}");
            }
        }

        // Crear un pago
        public async Task<string> CreatePayment(string accessToken, decimal totalAmount, string returnUrl, string cancelUrl)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Crear el cuerpo JSON de la solicitud
                string jsonBody = $"{{" +
                    "\"intent\": \"sale\"," +
                    "\"payer\": {{" +
                        "\"payment_method\": \"paypal\"" +
                    "}}," +
                    "\"transactions\": [{{" +
                        "\"amount\": {{" +
                        "\"total\": \"{totalAmount}\"," +
                        "\"currency\": \"USD\"" +
                        "}}," +
                        "\"description\": \"Compra en Marketplace\"" +
                    "}}]," +
                    "\"redirect_urls\": {{" +
                        "\"return_url\": \"{returnUrl}\"," +
                        "\"cancel_url\": \"{cancelUrl}\"" +
                    "}}" +
                "}}";

                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(_paypalUrl, content);
                string result = await response.Content.ReadAsStringAsync();

                // Procesar la respuesta y extraer la URL de aprobación de PayPal
                dynamic jsonResponse = JsonConvert.DeserializeObject(result);
                string approvalUrl = jsonResponse.links[1].href;

                return approvalUrl;
            }
        }

        //// Ejecutar el pago después de que el usuario lo autorice
        //public async Task ExecutePayment(string paymentId, string payerId, string accessToken)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //        string jsonBody = "{ \"payer_id\": \"" + payerId + "\" }";
        //        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        //        HttpResponseMessage response = await client.PostAsync($"https://api.sandbox.paypal.com/v1/payments/payment/{paymentId}/execute", content);
        //        string result = await response.Content.ReadAsStringAsync();
        //        // Aquí podrías guardar los detalles del pago en tu base de datos
        //    }
        public async Task ExecutePayment(string paymentId, string payerId, string accessToken)
        {
            using (var client = new HttpClient())
            {
                // Autenticación con el token de acceso de PayPal
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Cuerpo de la solicitud JSON para ejecutar el pago
                var jsonBody = new
                {
                    payer_id = payerId
                };

                // Convertir el objeto JSON a string
                var content = new StringContent(JsonConvert.SerializeObject(jsonBody), Encoding.UTF8, "application/json");

                // Realizar la solicitud POST a la API de PayPal
                HttpResponseMessage response = await client.PostAsync($"https://api.sandbox.paypal.com/v1/payments/payment/{paymentId}/execute", content);

                // Leer la respuesta
                string result = await response.Content.ReadAsStringAsync();

                // Verificar si la respuesta fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Aquí puedes hacer lo que sea necesario con la respuesta exitosa
                    // Ejemplo: guardar la transacción en tu base de datos
                    dynamic jsonResponse = JsonConvert.DeserializeObject(result);
                    string paymentStatus = jsonResponse.state; // Estado del pago (approved, etc.)
                    string transactionId = jsonResponse.transactions[0].related_resources[0].sale.id;

                    // Guardar detalles del pago en la base de datos
                    // Llama a tu método para guardar la transacción en la base de datos, etc.
                }
                else
                {
                    // Si la respuesta no es exitosa, manejar el error
                    // Puede ser útil registrar la respuesta para depuración
                    throw new Exception($"Error al ejecutar el pago: {result}");
                }
            }
        }

    }
}

