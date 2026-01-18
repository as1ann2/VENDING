using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VendingAPI.Models;

public partial class VendingprofContext : DbContext
{
    public VendingprofContext()
    {
    }

    public VendingprofContext(DbContextOptions<VendingprofContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CashCollection> CashCollections { get; set; }

    public virtual DbSet<MachineInventory> MachineInventories { get; set; }

    public virtual DbSet<MachineType> MachineTypes { get; set; }

    public virtual DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<ServiceOrder> ServiceOrders { get; set; }

    public virtual DbSet<StatusHistory> StatusHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VendingMachine> VendingMachines { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=1;Database=vendingprof");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CashCollection>(entity =>
        {
            entity.HasKey(e => e.CollectionId).HasName("cash_collections_pkey");

            entity.ToTable("cash_collections");

            entity.Property(e => e.CollectionId).HasColumnName("collection_id");
            entity.Property(e => e.CardAmount)
                .HasPrecision(10, 2)
                .HasColumnName("card_amount");
            entity.Property(e => e.CashAmount)
                .HasPrecision(10, 2)
                .HasColumnName("cash_amount");
            entity.Property(e => e.CollectionDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("collection_date");
            entity.Property(e => e.CollectorName)
                .HasMaxLength(100)
                .HasColumnName("collector_name");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasComputedColumnSql("(cash_amount + card_amount)", true)
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Machine).WithMany(p => p.CashCollections)
                .HasForeignKey(d => d.MachineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cash_collections_machine_id_fkey");
        });

        modelBuilder.Entity<MachineInventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("machine_inventory_pkey");

            entity.ToTable("machine_inventory");

            entity.HasIndex(e => new { e.MachineId, e.SlotNumber }, "machine_inventory_machine_id_slot_number_key").IsUnique();

            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.CurrentQuantity)
                .HasDefaultValue(0)
                .HasColumnName("current_quantity");
            entity.Property(e => e.LastRestocked).HasColumnName("last_restocked");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.MaxCapacity).HasColumnName("max_capacity");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.SellingPrice)
                .HasPrecision(8, 2)
                .HasColumnName("selling_price");
            entity.Property(e => e.SlotNumber).HasColumnName("slot_number");

            entity.HasOne(d => d.Machine).WithMany(p => p.MachineInventories)
                .HasForeignKey(d => d.MachineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("machine_inventory_machine_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.MachineInventories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("machine_inventory_product_id_fkey");
        });

        modelBuilder.Entity<MachineType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("machine_types_pkey");

            entity.ToTable("machine_types");

            entity.HasIndex(e => e.TypeName, "machine_types_type_name_key").IsUnique();

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.PaymentMethods)
                .HasMaxLength(50)
                .HasColumnName("payment_methods");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<MaintenanceRecord>(entity =>
        {
            entity.HasKey(e => e.MaintenanceId).HasName("maintenance_records_pkey");

            entity.ToTable("maintenance_records");

            entity.Property(e => e.MaintenanceId).HasColumnName("maintenance_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.MaintenanceDate).HasColumnName("maintenance_date");
            entity.Property(e => e.MaintenanceType)
                .HasMaxLength(20)
                .HasColumnName("maintenance_type");
            entity.Property(e => e.NextMaintenanceDate).HasColumnName("next_maintenance_date");
            entity.Property(e => e.PartsReplaced).HasColumnName("parts_replaced");
            entity.Property(e => e.Problems).HasColumnName("problems");
            entity.Property(e => e.TechnicianId).HasColumnName("technician_id");
            entity.Property(e => e.WorkDurationHours).HasColumnName("work_duration_hours");

            entity.HasOne(d => d.Machine).WithMany(p => p.MaintenanceRecords)
                .HasForeignKey(d => d.MachineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("maintenance_records_machine_id_fkey");

            entity.HasOne(d => d.Technician).WithMany(p => p.MaintenanceRecords)
                .HasForeignKey(d => d.TechnicianId)
                .HasConstraintName("maintenance_records_technician_id_fkey");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).HasName("manufacturers_pkey");

            entity.ToTable("manufacturers");

            entity.HasIndex(e => e.Name, "manufacturers_name_key").IsUnique();

            entity.Property(e => e.ManufacturerId).HasColumnName("manufacturer_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(100)
                .HasColumnName("contact_person");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("notifications_pkey");

            entity.ToTable("notifications");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.Machine).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.MachineId)
                .HasConstraintName("notifications_machine_id_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Barcode)
                .HasMaxLength(50)
                .HasColumnName("barcode");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasColumnName("category");
            entity.Property(e => e.CostPrice)
                .HasPrecision(8, 2)
                .HasColumnName("cost_price");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MinStockLevel)
                .HasDefaultValue(5)
                .HasColumnName("min_stock_level");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(8, 2)
                .HasColumnName("price");
            entity.Property(e => e.SalesTendency)
                .HasPrecision(8, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("sales_tendency");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.SaleId).HasName("sales_pkey");

            entity.ToTable("sales");

            entity.Property(e => e.SaleId).HasColumnName("sale_id");
            entity.Property(e => e.Amount)
                .HasPrecision(8, 2)
                .HasColumnName("amount");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(10)
                .HasColumnName("payment_method");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SaleDatetime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("sale_datetime");

            entity.HasOne(d => d.Machine).WithMany(p => p.Sales)
                .HasForeignKey(d => d.MachineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sales_machine_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.Sales)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sales_product_id_fkey");
        });

        modelBuilder.Entity<ServiceOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("service_orders_pkey");

            entity.ToTable("service_orders");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.AcceptedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("accepted_at");
            entity.Property(e => e.AssignedEngineerId).HasColumnName("assigned_engineer_id");
            entity.Property(e => e.CompletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("completed_at");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeadlineDate).HasColumnName("deadline_date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.OrderType)
                .HasMaxLength(20)
                .HasColumnName("order_type");
            entity.Property(e => e.Priority)
                .HasDefaultValue(1)
                .HasColumnName("priority");
            entity.Property(e => e.ScheduledDate).HasColumnName("scheduled_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Новая'::character varying")
                .HasColumnName("status");

            entity.HasOne(d => d.AssignedEngineer).WithMany(p => p.ServiceOrders)
                .HasForeignKey(d => d.AssignedEngineerId)
                .HasConstraintName("service_orders_assigned_engineer_id_fkey");

            entity.HasOne(d => d.Machine).WithMany(p => p.ServiceOrders)
                .HasForeignKey(d => d.MachineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("service_orders_machine_id_fkey");
        });

        modelBuilder.Entity<StatusHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("status_history_pkey");

            entity.ToTable("status_history");

            entity.Property(e => e.HistoryId).HasColumnName("history_id");
            entity.Property(e => e.ChangeReason).HasColumnName("change_reason");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changed_at");
            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.NewStatus)
                .HasMaxLength(30)
                .HasColumnName("new_status");
            entity.Property(e => e.OldStatus)
                .HasMaxLength(30)
                .HasColumnName("old_status");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.StatusHistories)
                .HasForeignKey(d => d.ChangedBy)
                .HasConstraintName("status_history_changed_by_fkey");

            entity.HasOne(d => d.Machine).WithMany(p => p.StatusHistories)
                .HasForeignKey(d => d.MachineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("status_history_machine_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
            entity.Property(e => e.Specialization).HasColumnName("specialization");
        });

        modelBuilder.Entity<VendingMachine>(entity =>
        {
            entity.HasKey(e => e.MachineId).HasName("vending_machines_pkey");

            entity.ToTable("vending_machines");

            entity.HasIndex(e => e.InventoryNumber, "vending_machines_inventory_number_key").IsUnique();

            entity.HasIndex(e => e.SerialNumber, "vending_machines_serial_number_key").IsUnique();

            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.CommissioningDate).HasColumnName("commissioning_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.InventoryDate).HasColumnName("inventory_date");
            entity.Property(e => e.InventoryNumber)
                .HasMaxLength(50)
                .HasColumnName("inventory_number");
            entity.Property(e => e.LastVerificationDate).HasColumnName("last_verification_date");
            entity.Property(e => e.LastVerificationEmployee).HasColumnName("last_verification_employee");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.ManufacturerId).HasColumnName("manufacturer_id");
            entity.Property(e => e.Model)
                .HasMaxLength(100)
                .HasColumnName("model");
            entity.Property(e => e.NextMaintenanceDate).HasColumnName("next_maintenance_date");
            entity.Property(e => e.ProductionCountry)
                .HasMaxLength(50)
                .HasColumnName("production_country");
            entity.Property(e => e.ProductionDate).HasColumnName("production_date");
            entity.Property(e => e.ResourceHours).HasColumnName("resource_hours");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(50)
                .HasColumnName("serial_number");
            entity.Property(e => e.ServiceTime).HasColumnName("service_time");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.TotalRevenue)
                .HasPrecision(15, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("total_revenue");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.VerificationInterval).HasColumnName("verification_interval");

            entity.HasOne(d => d.LastVerificationEmployeeNavigation).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.LastVerificationEmployee)
                .HasConstraintName("vending_machines_last_verification_employee_fkey");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vending_machines_manufacturer_id_fkey");

            entity.HasOne(d => d.Type).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vending_machines_type_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
