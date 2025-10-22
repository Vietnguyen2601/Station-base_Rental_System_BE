using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class VehicleModel
{
    public Guid VehicleModelId { get; set; }

    public Guid TypeId { get; set; }

    public string Name { get; set; } = null!;

    public string Manufacturer { get; set; } = null!;

    public decimal PricePerHour { get; set; }

    public string? Specs { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool Isactive { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual VehicleType Type { get; set; } = null!;

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
