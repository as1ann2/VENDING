using System;
using System.Collections.Generic;

namespace AvaloniaApplication.Models;

public partial class VendingMachine
{
    public int MachineId { get; set; }

    public string SerialNumber { get; set; } = null!;

    public string InventoryNumber { get; set; } = null!;

    public int TypeId { get; set; }

    public int ManufacturerId { get; set; }

    public string Model { get; set; } = null!;

    public string Location { get; set; } = null!;

    public DateOnly ProductionDate { get; set; }

    public DateOnly CommissioningDate { get; set; }

    public DateOnly? LastVerificationDate { get; set; }

    public int? VerificationInterval { get; set; }

    public int? ResourceHours { get; set; }

    public DateOnly? NextMaintenanceDate { get; set; }

    public int? ServiceTime { get; set; }

    public string? Status { get; set; }

    public string ProductionCountry { get; set; } = null!;

    public DateOnly? InventoryDate { get; set; }

    public int? LastVerificationEmployee { get; set; }

    public decimal? TotalRevenue { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CashCollection> CashCollections { get; set; } = new List<CashCollection>();

    public virtual User? LastVerificationEmployeeNavigation { get; set; }

    public virtual ICollection<MachineInventory> MachineInventories { get; set; } = new List<MachineInventory>();

    public virtual ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();

    public virtual ICollection<StatusHistory> StatusHistories { get; set; } = new List<StatusHistory>();

    public virtual MachineType Type { get; set; } = null!;
}
