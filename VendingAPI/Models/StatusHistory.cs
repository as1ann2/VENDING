using System;
using System.Collections.Generic;

namespace VendingAPI.Models;

public partial class StatusHistory
{
    public int HistoryId { get; set; }

    public int MachineId { get; set; }

    public string? OldStatus { get; set; }

    public string NewStatus { get; set; } = null!;

    public int? ChangedBy { get; set; }

    public string? ChangeReason { get; set; }

    public DateTime? ChangedAt { get; set; }

    public virtual User? ChangedByNavigation { get; set; }

    public virtual VendingMachine Machine { get; set; } = null!;
}
