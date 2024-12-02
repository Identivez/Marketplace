using Newtonsoft.Json;
using PruebaTaller.Models;

namespace GEJ_Lab.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ShoppingCart GetCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                var cart = new ShoppingCart();
                session.SetString("Cart", JsonConvert.SerializeObject(cart));
                return cart;
            }
            return JsonConvert.DeserializeObject<ShoppingCart>(cartJson);
        }

        public void SaveCart(ShoppingCart cart)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = JsonConvert.SerializeObject(cart);
            session.SetString("Cart", cartJson);
        }

        public void ClearCart()
        {
            _httpContextAccessor.HttpContext.Session.Remove("Cart");
        }
    }

}
