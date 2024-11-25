using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GEJ_Lab.Models
{
    public class ShippingDetails
    {
        /// <summary>
        /// Clase que representa los detalles del envío.
        /// </summary>

        [Key]
        public int ShippingDetailsId { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public virtual Order? Order { get; set; } // Puede ser anulable si no se requiere siempre la relación.
    }
}
