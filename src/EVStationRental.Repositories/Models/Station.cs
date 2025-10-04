using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class Station
{
    public Guid StationId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public decimal Lat { get; set; }

    public decimal Long { get; set; }

    public int Capacity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
