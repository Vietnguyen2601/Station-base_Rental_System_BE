using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class StaffRevenue
{
    public Guid StaffRevenueId { get; set; }

    public Guid StaffId { get; set; }

    public DateTime RevenueDate { get; set; }

    public decimal TotalRevenue { get; set; }

    public decimal? Commission { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Isactive { get; set; }

    public virtual Account Staff { get; set; } = null!;
}
