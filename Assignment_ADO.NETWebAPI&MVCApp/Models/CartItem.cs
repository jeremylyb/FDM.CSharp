using System;
using System.Collections.Generic;

namespace Assignment_ADO.NETWebAPI_MVCApp.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int ProductId { get; set; }

    public int FkCartId { get; set; }

    public virtual Cart FkCart { get; set; } = null!;
}
