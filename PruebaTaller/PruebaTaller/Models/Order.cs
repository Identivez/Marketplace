using PruebaTaller.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GEJ_Lab.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? OrderId { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; } = string.Empty;

        [Required]
        public virtual ApplicationUser User { get; set; } = null!;

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TotalAmount { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ShippingAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string? OrderStatus { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string? ShippingMethod { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? ShippingCost { get; set; }

        // Eliminamos la clave foránea aquí para evitar conflictos
        public virtual Payment? Payment { get; set; }

        public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
        public virtual ShippingDetails? ShippingDetails { get; set; }
    }
}
