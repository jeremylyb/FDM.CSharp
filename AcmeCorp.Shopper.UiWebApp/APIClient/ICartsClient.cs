
using AcmeCorp.Shopper.UiWebApp.Models;

namespace AcmeCorp.Shopper.UiWebApp.APIClient
{
    public interface ICartsClient
    {
        Task <CartItem?> AddProductToCart(int cartId, int productId);
        Task<List<CartItem>?> DeleteProductFromCart(int cartId, int productId);
        Task<CartDetailsDTO?> GetAllCartItemsForCartAsync(int? cartId);
        Task<Cart?> PostNewCart();
        Task<List<CartItem>?> DeleteAllProductFromCart(int cartId);
    }
}