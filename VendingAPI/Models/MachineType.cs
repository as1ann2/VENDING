using System;
using System.Collections.Generic;

namespace VendingAPI.Models;

public partial class MachineType
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public string? PaymentMethods { get; set; }

    public virtual ICollection<VendingMachine> VendingMachines { get; set; } = new List<VendingMachine>();
}
