using System;
using System.Collections.Generic;

namespace VendingAPI.Models;

public partial class CashCollection
{
    public int CollectionId { get; set; }

    public int MachineId { get; set; }

    public DateOnly CollectionDate { get; set; }

    public string CollectorName { get; set; } = null!;

    public decimal CashAmount { get; set; }

    public decimal CardAmount { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual VendingMachine Machine { get; set; } = null!;
}
