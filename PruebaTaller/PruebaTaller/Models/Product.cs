using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PruebaTaller.Models
{
    /// <summary>
    /// Clase que representa un producto en la tienda.
    /// Contiene información relacionada con el producto, como nombre, marca, categoría, precio, descripción, imagen y fecha de creación.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Obtiene o establece el ID del producto.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del producto.
        /// Longitud máxima de 100 caracteres.
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; } = "";

        /// <summary>
        /// Obtiene o establece la marca del producto.
        /// Longitud máxima de 100 caracteres.
        /// </summary>
        [MaxLength(100)]
        public string Brand { get; set; } = "";

        /// <summary>
        /// Obtiene o establece la categoría del producto.
        /// Longitud máxima de 100 caracteres.
        /// </summary>
        [MaxLength(100)]
        public string Category { get; set; } = "";

        /// <summary>
        /// Obtiene o establece el precio del producto con una precisión de 16 dígitos y 2 decimales.
        /// </summary>
        [Precision(16, 2)]
        public decimal Price { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del producto.
        /// Campo obligatorio.
        /// </summary>
        [Required]
        public string Description { get; set; } = "";

        /// <summary>
        /// Obtiene o establece el nombre del archivo de imagen asociado con el producto.
        /// Longitud máxima de 100 caracteres.
        /// </summary>
        [MaxLength(100)]
        public string ImageFileName { get; set; } = "";

        /// <summary>
        /// Obtiene o establece la fecha y hora de creación del producto.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
