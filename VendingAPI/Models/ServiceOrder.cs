using System;
using System.Collections.Generic;

namespace VendingAPI.Models;

public partial class ServiceOrder
{
    public int OrderId { get; set; }

    public int MachineId { get; set; }

    public int? AssignedEngineerId { get; set; }

    public string? OrderType { get; set; }

    public string? Status { get; set; }

    public DateOnly ScheduledDate { get; set; }

    public DateOnly? DeadlineDate { get; set; }

    public int? Priority { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual User? AssignedEngineer { get; set; }

    public virtual VendingMachine Machine { get; set; } = null!;
}
