using System;
using System.Collections.Generic;

namespace AvaloniaApplication.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public decimal Price { get; set; }

    public decimal? CostPrice { get; set; }

    public string? Barcode { get; set; }

    public int? MinStockLevel { get; set; }

    public decimal? SalesTendency { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<MachineInventory> MachineInventories { get; set; } = new List<MachineInventory>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}