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
        /// Obtiene o establece el ID del producto asociado con el artículo del carrito.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del producto.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o establece el precio unitario del producto.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del archivo de imagen asociado con el producto.
        /// </summary>
        public string ImageFileName { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad de unidades del producto en el carrito.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Obtiene el precio total del artículo en el carrito.
        /// Calcula el precio multiplicando el precio unitario por la cantidad.
        /// </summary>
        public decimal TotalPrice => Price * Quantity;
    }
}
