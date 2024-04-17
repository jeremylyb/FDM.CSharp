using System.Text;
using System.Text.Json.Serialization;

namespace AcmeCorp.Shopper.UiWebApp.Models
{
    public partial class CartDetailsDTO
    {

        [JsonPropertyName("cart")]
        public Cart Cart { get; set; }

        [JsonPropertyName("cartItemsWithProducts")]
        public List<CartItemWithProduct> CartItemsWithProducts { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CartId: {Cart.CartId}");
            sb.AppendLine($"CartPrice: {Cart.CartPrice}");

            sb.AppendLine("CartItems:");
            foreach (var cartItemWithProduct in CartItemsWithProducts)
            {
                sb.AppendLine(cartItemWithProduct.CartItem.CartItemId.ToString());
                sb.AppendLine(cartItemWithProduct.Product.ProductId.ToString());
                sb.AppendLine(cartItemWithProduct.Product.ProductName);
                sb.AppendLine(cartItemWithProduct.Product.ProductPrice.ToString());
            }

            return sb.ToString();
        }
    }
}
