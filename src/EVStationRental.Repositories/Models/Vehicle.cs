using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class Vehicle
{
    public Guid VehicleId { get; set; }

    public string SerialNumber { get; set; } = null!;

    public Guid ModelId { get; set; }

    public Guid? StationId { get; set; }

    public int? BatteryLevel { get; set; }

    public int? BatteryCapacity { get; set; }

    public int? Range { get; set; }

    public string? Color { get; set; }

    public DateOnly? LastMaintenance { get; set; }

    public string? Img { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool Isactive { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual VehicleModel Model { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual Station? Station { get; set; }
}
