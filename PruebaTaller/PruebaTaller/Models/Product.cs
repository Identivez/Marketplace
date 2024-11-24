using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PruebaTaller.Models
{
    /// <summary>
    /// Clase que representa un producto en la tienda.
    /// Contiene información relacionada con el producto, como nombre, marca, categoría, precio, descripción, imagen y fecha de creación.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Marca del producto.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        /// <summary>
        /// Precio del producto, con dos decimales.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(16, 2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Descripción detallada del producto.
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del archivo de imagen asociado con el producto.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ImageFileName { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de creación del registro del producto.
        /// Por defecto se inicializa con la fecha y hora actual.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Cantidad de stock disponible del producto.
        /// </summary>
        [Required]
        public int Stock { get; set; }
    }
}
