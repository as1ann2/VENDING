using System;
using System.Collections.Generic;

namespace AvaloniaApplication.Models;

public partial class MachineInventory
{
    public int InventoryId { get; set; }

    public int MachineId { get; set; }

    public int ProductId { get; set; }

    public int SlotNumber { get; set; }

    public int MaxCapacity { get; set; }

    public int? CurrentQuantity { get; set; }

    public decimal SellingPrice { get; set; }

    public DateOnly? LastRestocked { get; set; }

    public virtual VendingMachine Machine { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}