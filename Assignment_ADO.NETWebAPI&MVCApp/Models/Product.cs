using System;
using System.Collections.Generic;

namespace Assignment_ADO.NETWebAPI_MVCApp.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal ProductPrice { get; set; }
}
