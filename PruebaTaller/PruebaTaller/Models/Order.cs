using PruebaTaller.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GEJ_Lab.Models
{
    /// <summary>
    /// Clase que representa un pedido realizado por un usuario.
    /// Contiene información relacionada con el usuario, fecha del pedido, monto total, dirección de envío, estado del pedido, método y costo de envío, y detalles del pago.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Identificador único del pedido.
        /// </summary>
        [Key]
        public int OrderId { get; set; }

        /// <summary>
        /// Identificador del usuario que realizó el pedido.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Fecha en la que se realizó el pedido.
        /// </summary>
        [Required]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Monto total del pedido.
        /// </summary>
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Dirección de envío del pedido.
        /// </summary>
        [MaxLength(20)]
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;

        /// <summary>
        /// Estado del pedido, por ejemplo, "En proceso" o "Completado".
        /// </summary>
        [MaxLength(20)]
        [Required]
        public string OrderStatus { get; set; } = string.Empty;

        /// <summary>
        /// Identificador del pago asociado con el pedido.
        /// </summary>
        [Required]
        public int PaymentId { get; set; }

        /// <summary>
        /// Método de envío utilizado, por ejemplo, "Express" o "Estándar".
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string ShippingMethod { get; set; } = string.Empty;

        /// <summary>
        /// Costo del envío del pedido.
        /// </summary>
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ShippingCost { get; set; }

        /// <summary>
        /// Referencia al usuario que realizó el pedido.
        /// </summary>
        [ForeignKey("UserId")]
        [Required]
        public virtual ApplicationUser User { get; set; } = new ApplicationUser();

        /// <summary>
        /// Referencia al pago asociado con el pedido.
        /// </summary>
        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; } = new Payment();

        /// <summary>
        /// Colección de detalles del pedido, que incluye los productos comprados, cantidad y precio.
        /// </summary>
        [Required]
        public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    }
}
