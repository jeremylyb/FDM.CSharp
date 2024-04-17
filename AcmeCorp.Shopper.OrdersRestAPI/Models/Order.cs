using System;
using System.Collections.Generic;

namespace AcmeCorp.Shopper.OrdersRestAPI.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string CustomerName { get; set; } = null!;

    public int CartId { get; set; }
}
