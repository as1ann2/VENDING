using System;
using System.Collections.Generic;

namespace AvaloniaApplication.Models;

public partial class Sale
{
    public int SaleId { get; set; }

    public int MachineId { get; set; }

    public int ProductId { get; set; }

    public DateTime SaleDatetime { get; set; }

    public int Quantity { get; set; }

    public decimal Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public virtual VendingMachine Machine { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}