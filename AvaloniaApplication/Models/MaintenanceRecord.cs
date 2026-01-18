using System;
using System.Collections.Generic;

namespace AvaloniaApplication.Models;

public partial class MaintenanceRecord
{
    public int MaintenanceId { get; set; }

    public int MachineId { get; set; }

    public DateOnly MaintenanceDate { get; set; }

    public string? MaintenanceType { get; set; }

    public int? TechnicianId { get; set; }

    public string Description { get; set; } = null!;

    public string? Problems { get; set; }

    public string? PartsReplaced { get; set; }

    public int? WorkDurationHours { get; set; }

    public DateOnly? NextMaintenanceDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual VendingMachine Machine { get; set; } = null!;

    public virtual User? Technician { get; set; }
}