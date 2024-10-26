using System.ComponentModel.DataAnnotations;

namespace PruebaTaller.Models
{
    /// <summary>
    /// DTO (Data Transfer Object) utilizado para transferir información relacionada con la consulta de ventas por categoría.
    /// Contiene información sobre la categoría de productos, un indicador de resultados y una lista de productos encontrados.
    /// </summary>
    public class VentasDTO
    {
        /// <summary>
        /// Obtiene o establece la categoría de productos para realizar la búsqueda.
        /// Campo requerido.
        /// </summary>
        [Required]
        public string Category { get; set; } = "";

        /// <summary>
        /// Indica si se obtuvieron productos para la categoría especificada.
        /// Devuelve true si se encontraron productos, de lo contrario false.
        /// </summary>
        public bool WithData { get; set; } = false;

        /// <summary>
        /// Obtiene o establece la lista de productos encontrados para la categoría especificada.
        /// </summary>
        public List<ProductosDTO>? Products { get; set; }
    }
}
