using System;
using System.Collections.Generic;

namespace AcmeCorp.Shopper.CartsRestAPI.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int ProductId { get; set; }

    public int FkCartId { get; set; }

    public virtual Cart FkCart { get; set; } = null!;
}
