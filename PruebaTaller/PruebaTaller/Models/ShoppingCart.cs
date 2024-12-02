using System.Linq;

namespace PruebaTaller.Models
{
    /// <summary>
    /// Clase que representa un carrito de compras.
    /// Permite gestionar la lista de artículos en el carrito y realizar operaciones como agregar, eliminar o actualizar artículos.
    /// </summary>
    public class ShoppingCart
    {
        /// <summary>
        /// Obtiene o establece la lista de artículos en el carrito de compras.
        /// </summary>
        public List<Cart_Item> Items { get; set; } = new List<Cart_Item>();

        /// <summary>
        /// Añade un artículo al carrito de compras.
        /// Si el artículo ya existe en el carrito, incrementa su cantidad.
        /// </summary>
        /// <param name="item">El artículo a agregar al carrito.</param>
        public void AddItem(Cart_Item item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity; // Incrementar cantidad si ya existe
                existingItem.TotalPrice = existingItem.Quantity * existingItem.Price; // Recalcula el total
            }
            else
            {
                item.TotalPrice = item.Quantity * item.Price; // Calcula el total del nuevo ítem
                Items.Add(item);
            }
        }

        /// <summary>
        /// Obtiene la lista de artículos en el carrito.
        /// </summary>
        /// <returns>Lista de artículos en el carrito.</returns>
        public List<Cart_Item> GetItems()
        {
            return Items;
        }

        /// <summary>
        /// Elimina un artículo del carrito de compras.
        /// Si la cantidad del artículo es mayor a 1, decrementa su cantidad; si es 1, lo elimina del carrito.
        /// </summary>
        /// <param name="productId">El ID del producto que se desea eliminar o decrementar.</param>
        public void RemoveItem(int productId)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                Items.Remove(existingItem); // Elimina el ítem del carrito
            }
        }

        /// <summary>
        /// Actualiza la cantidad de un artículo en el carrito.
        /// Si la cantidad establecida es 0, elimina el artículo del carrito.
        /// </summary>
        /// <param name="productId">El ID del producto que se desea actualizar.</param>
        /// <param name="quantity">La nueva cantidad para el producto.</param>
        public void UpdateItemQuantity(int productId, int quantity)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null && quantity > 0)
            {
                existingItem.Quantity = quantity;
                existingItem.TotalPrice = existingItem.Quantity * existingItem.Price; // Recalcula el total
            }
            else if (existingItem != null && quantity == 0)
            {
                RemoveItem(productId);
            }
        }

        /// <summary>
        /// Calcula el subtotal de los artículos en el carrito (sin incluir costos adicionales como envío).
        /// </summary>
        /// <returns>El subtotal de los artículos en el carrito.</returns>
        public decimal CalculateSubtotal()
        {
            return Items.Sum(i => i.Quantity * i.Price);
        }

        /// <summary>
        /// Calcula el total general del carrito, incluyendo un costo adicional fijo.
        /// </summary>
        /// <param name="shippingCost">El costo de envío (opcional, por defecto 0).</param>
        /// <returns>El total general del carrito.</returns>
        public decimal CalculateTotal(decimal shippingCost = 0)
        {
            return CalculateSubtotal() + shippingCost;
        }

        /// <summary>
        /// Vacía todos los artículos del carrito.
        /// </summary>
        public void ClearCart()
        {
            Items.Clear();
        }

        /// <summary>
        /// Obtiene la cantidad total de artículos en el carrito (suma de cantidades de todos los productos).
        /// </summary>
        /// <returns>La cantidad total de artículos en el carrito.</returns>
        public int GetTotalItemCount()
        {
            return Items.Sum(i => i.Quantity);
        }
    }
}
