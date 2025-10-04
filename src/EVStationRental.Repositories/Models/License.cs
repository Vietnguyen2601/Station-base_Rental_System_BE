using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class License
{
    public Guid LicenseId { get; set; }

    public Guid AccountId { get; set; }

    public string LicenseImageUrl { get; set; } = null!;

    public string IdentityCardImageUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Account Account { get; set; } = null!;
}
