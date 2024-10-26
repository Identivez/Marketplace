using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PruebaTaller.Models;
using System.Net.Http;

namespace PruebaTaller.Controllers
{
    /// <summary>
    /// Controlador que maneja las utilidades para interactuar con API externas y mostrar ejemplos.
    /// Permite realizar consultas a la API de universidades y devolver los resultados en una vista.
    /// </summary>
    public class UtilitiesController : Controller
    {
        // Cliente HTTP para realizar las solicitudes a APIs externas.
        private readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Muestra la vista de ejemplos de utilidades.
        /// </summary>
        /// <returns>Vista de ejemplos de utilidades.</returns>
        public IActionResult Examples()
        {
            return View();
        }

        /// <summary>
        /// Realiza una solicitud a la API externa de universidades utilizando el país proporcionado.
        /// </summary>
        /// <param name="utilitiesDTO">DTO que contiene el país para la búsqueda y los resultados de la API.</param>
        /// <returns>Vista con los resultados de la búsqueda de universidades o el formulario con errores de validación.</returns>
        [HttpPost]
        public async Task<IActionResult> Examples(UtilitiesDTO utilitiesDTO)
        {
            string jsonString = "";

            // Validar el modelo proporcionado
            if (!ModelState.IsValid)
            {
                return View(utilitiesDTO);
            }

            // Definir la URL base para la API de universidades
            string baseUrl = "http://universities.hipolabs.com";
            var url = $"{baseUrl}/search?country={utilitiesDTO.Pais}";

            // Realizar la solicitud GET a la API
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            // Verificar si la solicitud fue exitosa
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                // Leer la respuesta de la API como una cadena JSON
                jsonString = await response.Content.ReadAsStringAsync();

                // Deserializar la cadena JSON a una lista de objetos UniversitiesDTO
                utilitiesDTO.Universities = JsonConvert.DeserializeObject<List<UniversitiesDTO>>(jsonString);

                // Verificar si se recibieron datos de universidades
                if (utilitiesDTO.Universities == null || utilitiesDTO.Universities.Count == 0)
                {
                    utilitiesDTO.WithData = false;
                }
                else
                {
                    utilitiesDTO.WithData = true;
                }

                return View(utilitiesDTO);
            }

            // Si la solicitud no fue exitosa, devolver el modelo con datos de validación
            return View(utilitiesDTO);
        }
    }
}
