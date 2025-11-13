using EVStationRental.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.DBContext;

public partial class ElectricVehicleDBContext : DbContext
{
    public ElectricVehicleDBContext()
    {
    }

    public ElectricVehicleDBContext(DbContextOptions<ElectricVehicleDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<DamageReport> DamageReports { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StaffRevenue> StaffRevenues { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleModel> VehicleModels { get; set; }

    public virtual DbSet<VehicleType> VehicleTypes { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<WalletTransaction> WalletTransactions { get; set; }

    public static string GetConnectionString(string connectionStringName)
    {
        // Kiểm tra biến môi trường trước
        string envConnectionString = Environment.GetEnvironmentVariable($"ConnectionStrings__{connectionStringName}");
        if (!string.IsNullOrEmpty(envConnectionString))
        {
            return envConnectionString;
        }

        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(GetConnectionString("DefaultConnection"))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("damage_level_enum", new[] { "MINOR", "MODERATE", "SEVERE" })
            .HasPostgresEnum("order_status", new[] { "PENDING", "CONFIRMED", "ONGOING", "COMPLETED", "CANCELED" })
            .HasPostgresEnum("payment_type_enum", new[] { "DEPOSIT", "FINAL", "REFUND" })
            .HasPostgresEnum("transaction_type_enum", new[] { "DEPOSIT", "PAYMENT", "REFUND" })
            .HasPostgresEnum("vehicle_status", new[] { "AVAILABLE", "RENTED", "MAINTENANCE", "CHARGING" })
            .HasPostgresExtension("pgcrypto")
            .HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("Accounts_pkey");

            entity.Property(e => e.AccountId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("account_id");
            entity.Property(e => e.ContactNumber)
                .HasColumnType("character varying")
                .HasColumnName("contact_number");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasColumnType("character varying")
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Accounts_role_id_fkey");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("Contracts_pkey");

            entity.Property(e => e.ContractId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("contract_id");
            entity.Property(e => e.ContractDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("contract_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.FileUrl)
                .HasColumnType("character varying")
                .HasColumnName("file_url");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Contracts_customer_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Contracts_order_id_fkey");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Contracts_vehicle_id_fkey");
        });

        modelBuilder.Entity<DamageReport>(entity =>
        {
            entity.HasKey(e => e.DamageId).HasName("DamageReports_pkey");

            entity.Property(e => e.DamageId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("damage_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EstimatedCost).HasColumnName("estimated_cost");
            entity.Property(e => e.DamageLevel)
                .HasColumnName("damage_level");
            entity.Property(e => e.Img)
                .HasColumnType("character varying")
                .HasColumnName("img");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Order).WithMany(p => p.DamageReports)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("DamageReports_order_id_fkey");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.DamageReports)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("DamageReports_vehicle_id_fkey");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("Feedbacks_pkey");

            entity.Property(e => e.FeedbackId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("feedback_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.FeedbackDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("feedback_date");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Customer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Feedbacks_customer_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Feedbacks_order_id_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("Orders_pkey");

            entity.HasIndex(e => e.OrderCode, "Orders_order_code_key").IsUnique();

            entity.Property(e => e.OrderId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("order_id");
            entity.Property(e => e.BasePrice).HasColumnName("base_price");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.EndTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_time");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.OrderCode)
                .HasColumnType("character varying")
                .HasColumnName("order_code");
            entity.Property(e => e.OrderDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("order_date");
            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.ReturnTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("return_time");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.StartTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice).HasColumnName("total_price");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.OrderCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Orders_customer_id_fkey");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PromotionId)
                .HasConstraintName("Orders_promotion_id_fkey");

            entity.HasOne(d => d.Staff).WithMany(p => p.OrderStaffs)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("Orders_staff_id_fkey");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Orders)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Orders_vehicle_id_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("Payments_pkey");

            entity.HasIndex(e => e.GatewayTxId, "ux_payments_gateway_tx_id")
                .IsUnique()
                .HasFilter("(gateway_tx_id IS NOT NULL)");

            entity.HasIndex(e => e.IdempotencyKey, "ux_payments_idempotency")
                .IsUnique()
                .HasFilter("(idempotency_key IS NOT NULL)");

            entity.Property(e => e.PaymentId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("payment_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.GatewayResponse)
                .HasColumnType("jsonb")
                .HasColumnName("gateway_response");
            entity.Property(e => e.GatewayTxId)
                .HasColumnType("character varying")
                .HasColumnName("gateway_tx_id");
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(255)
                .HasColumnName("idempotency_key");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasColumnType("character varying")
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentType)
                .HasColumnName("payment_type");
            entity.Property(e => e.Status)
                .HasColumnType("character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Payments_order_id_fkey");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("Promotions_pkey");

            entity.Property(e => e.PromotionId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("promotion_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DiscountPercentage).HasColumnName("discount_percentage");
            entity.Property(e => e.EndDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_date");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.PromoCode)
                .HasColumnType("character varying")
                .HasColumnName("promo_code");
            entity.Property(e => e.StartDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_date");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("Reports_pkey");

            entity.Property(e => e.ReportId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("report_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.GeneratedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("generated_date");
            entity.Property(e => e.Img)
                .HasColumnType("character varying")
                .HasColumnName("img");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.ReportType)
                .HasColumnType("character varying")
                .HasColumnName("report_type");
            entity.Property(e => e.Text)
                .HasColumnType("character varying")
                .HasColumnName("text");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Account).WithMany(p => p.Reports)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Reports_account_id_fkey");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Reports)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Reports_vehicle_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("Roles_pkey");

            entity.Property(e => e.RoleId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("role_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.RoleName)
                .HasColumnType("character varying")
                .HasColumnName("role_name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<StaffRevenue>(entity =>
        {
            entity.HasKey(e => e.StaffRevenueId).HasName("Staff_Revenues_pkey");

            entity.ToTable("Staff_Revenues");

            entity.Property(e => e.StaffRevenueId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("staff_revenue_id");
            entity.Property(e => e.Commission).HasColumnName("commission");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.RevenueDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("revenue_date");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.TotalRevenue).HasColumnName("total_revenue");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Staff).WithMany(p => p.StaffRevenues)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Staff_Revenues_staff_id_fkey");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.StationId).HasName("Stations_pkey");

            entity.Property(e => e.StationId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("station_id");
            entity.Property(e => e.Address)
                .HasColumnType("character varying")
                .HasColumnName("address");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl)
                .HasColumnType("character varying")
                .HasColumnName("image_url");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Lat).HasColumnName("lat");
            entity.Property(e => e.Long).HasColumnName("long");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("Vehicles_pkey");

            entity.HasIndex(e => e.SerialNumber, "Vehicles_serial_number_key").IsUnique();

            entity.Property(e => e.VehicleId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("vehicle_id");
            entity.Property(e => e.BatteryCapacity).HasColumnName("battery_capacity");
            entity.Property(e => e.BatteryLevel).HasColumnName("battery_level");
            entity.Property(e => e.Color)
                .HasColumnType("character varying")
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Img)
                .HasColumnType("character varying")
                .HasColumnName("img");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.LastMaintenance).HasColumnName("last_maintenance");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.Range).HasColumnName("range");
            entity.Property(e => e.SerialNumber)
                .HasColumnType("character varying")
                .HasColumnName("serial_number");
            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.Status)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Model).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Vehicles_model_id_fkey");

            entity.HasOne(d => d.Station).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("Vehicles_station_id_fkey");
        });

        modelBuilder.Entity<VehicleModel>(entity =>
        {
            entity.HasKey(e => e.VehicleModelId).HasName("VehicleModels_pkey");

            entity.Property(e => e.VehicleModelId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("vehicle_model_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Manufacturer)
                .HasColumnType("character varying")
                .HasColumnName("manufacturer");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.PricePerHour).HasColumnName("price_per_hour");
            entity.Property(e => e.Specs)
                .HasColumnType("character varying")
                .HasColumnName("specs");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Type).WithMany(p => p.VehicleModels)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("VehicleModels_type_id_fkey");
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.HasKey(e => e.VehicleTypeId).HasName("VehicleTypes_pkey");

            entity.HasIndex(e => e.TypeName, "VehicleTypes_type_name_key").IsUnique();

            entity.Property(e => e.VehicleTypeId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("vehicle_type_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.TypeName)
                .HasColumnType("character varying")
                .HasColumnName("type_name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.WalletId).HasName("Wallets_pkey");

            entity.Property(e => e.WalletId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("wallet_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Balance).HasColumnName("balance");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Account).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Wallets_account_id_fkey");
        });

        modelBuilder.Entity<WalletTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("WalletTransactions_pkey");

            entity.Property(e => e.TransactionId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("transaction_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TransactionType)
                .HasColumnName("transaction_type");
            entity.Property(e => e.WalletId).HasColumnName("wallet_id");

            entity.HasOne(d => d.Order).WithMany(p => p.WalletTransactions)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("WalletTransactions_order_id_fkey");

            entity.HasOne(d => d.Wallet).WithMany(p => p.WalletTransactions)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("WalletTransactions_wallet_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
