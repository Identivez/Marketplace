using PruebaTaller.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GEJ_Lab.Models
{
    /// <summary>
    /// Clase que representa los detalles de un pedido.
    /// Contiene información sobre el pedido, el producto, la cantidad, el precio unitario y un cupón de descuento opcional.
    /// </summary>
    public class OrderDetails
    {
        /// <summary>
        /// Identificador único de los detalles del pedido.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailsId { get; set; }

        /// <summary>
        /// Identificador del pedido asociado a estos detalles.
        /// </summary>
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        /// <summary>
        /// Referencia al pedido asociado a estos detalles.
        /// </summary>
        public virtual Order? Order { get; set; }

        /// <summary>
        /// Identificador del producto asociado con el pedido.
        /// </summary>
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        /// <summary>
        /// Referencia al producto asociado con los detalles del pedido.
        /// </summary>
        public virtual Product? Product { get; set; }

        /// <summary>
        /// Cantidad de unidades del producto en el pedido.
        /// </summary>
        [Required]
        public int Quantity { get; set; }

        /// <summary>
        /// Precio unitario del producto.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Código del cupón de descuento, si se aplicó alguno.
        /// </summary>
        [MaxLength(10)]
        public string? DiscountVoucher { get; set; }
    }
}
