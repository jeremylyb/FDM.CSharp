using System;
using System.Collections.Generic;
using System.Text;

namespace AcmeCorp.Shopper.CartsRestAPI.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public decimal? CartPrice { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"CartId: {CartId}");
        sb.AppendLine($"CartPrice: {CartPrice}");

        sb.AppendLine("CartItems:");
        foreach (var cartItem in CartItems)
        {
            sb.AppendLine(cartItem.ToString());
        }

        return sb.ToString();
    }
}
