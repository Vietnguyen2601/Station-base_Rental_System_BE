﻿using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class Promotion
{
    public Guid PromotionId { get; set; }

    public string PromoCode { get; set; } = null!;

    public decimal DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? ApplicableTo { get; set; }

    public Guid? StationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Station? Station { get; set; }
}
