namespace PruebaTaller.Models
{
    /// <summary>
    /// DTO (Data Transfer Object) utilizado para transferir información de productos desde una fuente externa.
    /// Incluye datos básicos del producto como título, descripción, precio, imagen y categoría.
    /// </summary>
    public class ProductosDTO
    {
        /// <summary>
        /// Obtiene o establece el título o nombre del producto.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del producto.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Obtiene o establece el precio del producto.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Obtiene o establece la URL de la imagen del producto.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Obtiene o establece la categoría del producto.
        /// </summary>
        public string Category { get; set; }
    }
}
