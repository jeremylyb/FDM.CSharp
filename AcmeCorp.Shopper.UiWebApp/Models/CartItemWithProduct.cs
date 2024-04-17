using System.Text.Json.Serialization;

namespace AcmeCorp.Shopper.UiWebApp.Models
{
    public partial class CartItemWithProduct
    {
        [JsonPropertyName("cartItem")]
        public CartItem CartItem { get; set; }

        [JsonPropertyName("product")]
        public Product Product { get; set; }
    }
}
