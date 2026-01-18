using System;
using System.Collections.Generic;

namespace AvaloniaApplication.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? MachineId { get; set; }

    public string? Type { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual VendingMachine? Machine { get; set; }
}