using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaTaller.Services;

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
        /// Si el artículo ya existe, incrementa su cantidad.
        /// </summary>
        /// <param name="item">El artículo a agregar al carrito.</param>
        public void AddItem(Cart_Item item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity; // Incrementar cantidad si ya existe
            }
            else
            {
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
        /// Elimina un artículo del carrito de compras o decrementa su cantidad.
        /// Si la cantidad es 1, elimina el artículo del carrito.
        /// </summary>
        /// <param name="productId">El ID del producto que se desea eliminar o decrementar.</param>
        public void RemoveItem(int productId)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                if (existingItem.Quantity > 1)
                {
                    existingItem.Quantity--; // Decrementa la cantidad
                }
                else
                {
                    Items.Remove(existingItem); // Elimina el ítem si la cantidad es 1
                }
            }
        }

        /// <summary>
        /// Actualiza la cantidad de un artículo en el carrito.
        /// Si la cantidad es 0, elimina el artículo del carrito.
        /// </summary>
        /// <param name="productId">El ID del producto que se desea actualizar.</param>
        /// <param name="quantity">La nueva cantidad para el producto.</param>
        public void UpdateItemQuantity(int productId, int quantity)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null && quantity > 0)
            {
                existingItem.Quantity = quantity;
            }
            else if (existingItem != null && quantity == 0)
            {
                RemoveItem(productId);
            }
        }
    }
}
