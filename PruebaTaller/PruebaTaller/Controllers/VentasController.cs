using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PruebaTaller.Models;
using System.Net.Http;

namespace PruebaTaller.Controllers
{
    /// <summary>
    /// Controlador para gestionar la visualización de ventas.
    /// Permite mostrar una lista de productos según la categoría seleccionada utilizando una API externa.
    /// </summary>
    public class VentasController : Controller
    {
        // Cliente HTTP para realizar solicitudes a APIs externas.
        private readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Muestra la vista principal de ventas.
        /// </summary>
        /// <returns>Vista para la selección de una categoría de productos.</returns>
        public IActionResult Ventas()
        {
            return View();
        }

        /// <summary>
        /// Realiza una solicitud a una API externa para obtener productos según la categoría proporcionada.
        /// </summary>
        /// <param name="ventasDTO">DTO que contiene la categoría seleccionada y los resultados de la API.</param>
        /// <returns>Vista con los resultados de la búsqueda de productos o el formulario con errores de validación.</returns>
        [HttpPost]
        public async Task<IActionResult> Ventas(VentasDTO ventasDTO)
        {
            string jsonString = "";

            // Validar el modelo proporcionado
            if (!ModelState.IsValid)
            {
                return View(ventasDTO);
            }

            // Definir la URL base para la API de productos
            string baseUrl = "https://fakestoreapi.com";
            var url = $"{baseUrl}/products/category/{ventasDTO.Category}";

            // Realizar la solicitud GET a la API
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            // Verificar si la solicitud fue exitosa
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                // Leer la respuesta de la API como una cadena JSON
                jsonString = await response.Content.ReadAsStringAsync();

                // Deserializar la cadena JSON a una lista de objetos ProductosDTO
                ventasDTO.Products = JsonConvert.DeserializeObject<List<ProductosDTO>>(jsonString);

                // Verificar si se recibieron datos de productos
                if (ventasDTO.Products == null || ventasDTO.Products.Count == 0)
                {
                    ventasDTO.WithData = false;
                }
                else
                {
                    ventasDTO.WithData = true;
                }

                return View(ventasDTO);
            }

            // Si la solicitud no fue exitosa, devolver el modelo con datos de validación
            return View(ventasDTO);
        }
    }
}
