using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AcmeCorp.Shopper.CartsRestAPI.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int ProductId { get; set; }

    public int FkCartId { get; set; }

    [JsonIgnore]
    public virtual Cart FkCart { get; set; } = null!;
}
