using PruebaTaller.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GEJ_Lab.Models
{
    /// <summary>
    /// Clase que representa una reseña de un producto realizada por un usuario.
    /// Contiene información como el producto, el usuario que realizó la reseña, la calificación, el comentario y la fecha de la reseña.
    /// </summary>
    public class Review
    {
        /// <summary>
        /// Identificador único de la reseña.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewId { get; set; }

        /// <summary>
        /// Identificador del producto asociado con la reseña.
        /// </summary>
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        /// <summary>
        /// Referencia al producto asociado con la reseña.
        /// </summary>
        public virtual Product? Product { get; set; }

        /// <summary>
        /// Identificador del usuario que realizó la reseña.
        /// </summary>
        [ForeignKey("User")]
        public int UserId { get; set; }

        /// <summary>
        /// Referencia al usuario que realizó la reseña.
        /// </summary>
        public virtual ApplicationUser? User { get; set; }

        /// <summary>
        /// Calificación del producto, entre 1 y 5.
        /// </summary>
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        /// <summary>
        /// Comentario opcional del usuario sobre el producto.
        /// </summary>
        [MaxLength(1000)]
        public string? Comment { get; set; }

        /// <summary>
        /// Fecha en la que se realizó la reseña.
        /// </summary>
        [Required]
        public DateTime ReviewDate { get; set; }
    }
}
