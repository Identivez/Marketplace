using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PruebaTaller.Models
{
    /// <summary>
    /// Clase que representa un artículo en el carrito de compras.
    /// Contiene información del producto, incluyendo su ID, nombre, precio, cantidad y nombre de archivo de imagen.
    /// También calcula el precio total del artículo según la cantidad.
    /// </summary>
    public class Cart_Item
    {
        /// <summary>
        /// Identificador del usuario al que pertenece el artículo del carrito.
        /// </summary>
        [ForeignKey("User")]
        public int UserId { get; set; }

        /// <summary>
        /// Referencia al usuario asociado con el artículo del carrito.
        /// </summary>
        public virtual ApplicationUser? User { get; set; }

        /// <summary>
        /// Identificador del producto asociado con el artículo del carrito.
        /// </summary>
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        /// <summary>
        /// Referencia al producto asociado con el artículo del carrito.
        /// </summary>
        public virtual Product? Product { get; set; }

        /// <summary>
        /// Nombre del producto asociado con el artículo del carrito.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Precio unitario del producto.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Cantidad del producto que se está comprando.
        /// </summary>
        [Required]
        public int Quantity { get; set; }

        /// <summary>
        /// Nombre del archivo de imagen del producto asociado con el artículo del carrito.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ImageFileName { get; set; } = string.Empty;

        /// <summary>
        /// Precio total del artículo en el carrito, calculado como precio unitario por cantidad.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalPrice { get; set; }
    }
}
