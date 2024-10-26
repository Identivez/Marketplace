using System.ComponentModel.DataAnnotations;

namespace PruebaTaller.Models
{
    /// <summary>
    /// DTO (Data Transfer Object) utilizado para realizar consultas sobre universidades por país.
    /// Contiene información sobre el país de búsqueda, un indicador de resultados y una lista de universidades encontradas.
    /// </summary>
    public class UtilitiesDTO
    {
        /// <summary>
        /// Obtiene o establece el nombre del país para realizar la búsqueda de universidades.
        /// Campo requerido.
        /// </summary>
        [Required]
        public string Pais { get; set; } = "";

        /// <summary>
        /// Indica si se obtuvieron datos de universidades en la búsqueda.
        /// Devuelve true si se encontraron datos, de lo contrario false.
        /// </summary>
        public bool WithData { get; set; } = false;

        /// <summary>
        /// Obtiene o establece la lista de universidades encontradas para el país especificado.
        /// </summary>
        public List<UniversitiesDTO>? Universities { get; set; }
    }
}
