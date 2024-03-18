
using System.Text.Json.Serialization;


namespace AcmeCorp.Shopper.UiWebApp.Models;

public partial class Product
{
    [JsonPropertyName("productId")]
    public int ProductId { get; set; }

    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = null!;

    [JsonPropertyName("productPrice")]
    public decimal ProductPrice { get; set; }

    public override string ToString()
    {
        return $"Product = [ProductId = {ProductId}, ProductName = {ProductName}, ProductPrice = {ProductPrice}]";
    }
}
