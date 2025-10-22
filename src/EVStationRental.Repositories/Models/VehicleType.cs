using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class VehicleType
{
    public Guid VehicleTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool Isactive { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<VehicleModel> VehicleModels { get; set; } = new List<VehicleModel>();
}
