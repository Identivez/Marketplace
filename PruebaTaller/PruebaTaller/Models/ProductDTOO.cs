using System.ComponentModel.DataAnnotations;

namespace PruebaTaller.Models
{
    /// <summary>
    /// DTO (Data Transfer Object) utilizado para transferir información de productos.
    /// Incluye información relevante del producto como nombre, marca, categoría, precio, descripción, archivo de imagen y fecha de creación.
    /// </summary>
    public class ProductDTOO
    {
        /// <summary>
        /// Obtiene o establece el ID del producto.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del producto.
        /// Campo requerido con una longitud máxima de 100 caracteres.
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        /// <summary>
        /// Obtiene o establece la marca del producto.
        /// Campo requerido con una longitud máxima de 100 caracteres.
        /// </summary>
        [Required, MaxLength(100)]
        public string Brand { get; set; } = "";

        /// <summary>
        /// Obtiene o establece la categoría del producto.
        /// Campo requerido con una longitud máxima de 100 caracteres.
        /// </summary>
        [Required, MaxLength(100)]
        public string Category { get; set; } = "";

        /// <summary>
        /// Obtiene o establece el precio del producto.
        /// Campo requerido.
        /// </summary>
        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del producto.
        /// Campo requerido.
        /// </summary>
        [Required]
        public string Description { get; set; } = "";

        /// <summary>
        /// Obtiene o establece el archivo de imagen del producto.
        /// Campo opcional.
        /// </summary>
        public IFormFile? ImageFile { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha y hora de creación del producto.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
