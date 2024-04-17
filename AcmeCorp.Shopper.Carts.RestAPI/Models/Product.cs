using System;
using System.Collections.Generic;

namespace AcmeCorp.Shopper.CartsRestAPI.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal ProductPrice { get; set; }
}
