using PruebaTaller.Models;

namespace GEJ_Lab.Models
{
    public class ShippingDetailsDTO
    {
        /// <summary>
        /// Obtiene o establece el título o nombre del producto.
        /// </summary>
        public List<Cart_Item>? Items { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del producto.
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Obtiene o establece el precio del producto.
        /// </summary>
        public decimal ShippingCost { get; set; }

        /// <summary>
        /// Obtiene o establece la URL de la imagen del producto.
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
