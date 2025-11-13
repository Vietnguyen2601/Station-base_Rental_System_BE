using EVStationRental.Common.Enums.EnumModel;
using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class DamageReport
{
    public Guid DamageId { get; set; }

    public Guid OrderId { get; set; }

    public Guid VehicleId { get; set; }

    public string Description { get; set; } = null!;

    public decimal EstimatedCost { get; set; }

    public string? Img { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Isactive { get; set; }

    public DamageLevelEnum DamageLevel { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
