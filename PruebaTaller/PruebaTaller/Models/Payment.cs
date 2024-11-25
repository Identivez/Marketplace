using PruebaTaller.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GEJ_Lab.Models
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        // Clave foránea hacia `Order`
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public virtual ApplicationUser User { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(20)]
        public string PaymentStatus { get; set; } = string.Empty;

        [Required]
        public DateTime TransactionDate { get; set; }

        [MaxLength(50)]
        public string? PaymentProvider { get; set; }
    }
}
