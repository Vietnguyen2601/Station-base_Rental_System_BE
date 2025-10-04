using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class Contract
{
    public Guid ContractId { get; set; }

    public Guid OrderId { get; set; }

    public DateTime ContractDate { get; set; }

    public string Terms { get; set; } = null!;

    public string? Signature { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Order Order { get; set; } = null!;
}
