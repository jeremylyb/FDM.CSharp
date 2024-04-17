using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AcmeCorp.Shopper.UiWebApp.Models;

public partial class Order
{
    //[JsonIgnore]
    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }
    [JsonPropertyName("customerName")]
    public string CustomerName { get; set; } = null!;

    [JsonPropertyName("cartId")]
    public int CartId { get; set; }
}
