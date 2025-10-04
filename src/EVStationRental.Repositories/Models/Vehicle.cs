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

    public decimal? LocationLat { get; set; }

    public decimal? LocationLong { get; set; }

    public DateOnly? LastMaintenance { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual VehicleModel Model { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Station? Station { get; set; }
}
