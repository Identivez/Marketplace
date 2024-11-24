using PruebaTaller.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GEJ_Lab.Models
{
    /// <summary>
    /// Clase que representa un pago realizado por un usuario para una orden específica.
    /// Contiene información sobre la orden, el usuario, el método de pago, el monto, el estado del pago y la fecha de la transacción.
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// Identificador único del pago.
        /// </summary>
        [Key]
        public int PaymentId { get; set; }

        /// <summary>
        /// Identificador de la orden asociada al pago.
        /// </summary>
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        /// <summary>
        /// Referencia a la orden asociada al pago.
        /// </summary>
        public virtual Order? Order { get; set; }

        /// <summary>
        /// Identificador del usuario que realizó el pago.
        /// </summary>
        [ForeignKey("User")]
        public int UserId { get; set; }

        /// <summary>
        /// Referencia al usuario que realizó el pago.
        /// </summary>
        public virtual ApplicationUser? User { get; set; }

        /// <summary>
        /// Método de pago utilizado, por ejemplo, "Tarjeta de crédito" o "PayPal".
        /// </summary>
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        /// <summary>
        /// Monto del pago realizado.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Estado del pago, por ejemplo, "Completado" o "Pendiente".
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string PaymentStatus { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora de la transacción del pago.
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Proveedor del servicio de pago, opcional, por ejemplo, "Visa" o "MasterCard".
        /// </summary>
        [MaxLength(50)]
        public string? PaymentProvider { get; set; }
    }
}
