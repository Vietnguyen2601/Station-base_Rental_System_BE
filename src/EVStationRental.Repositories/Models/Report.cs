using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class Report
{
    public Guid ReportId { get; set; }

    public string ReportType { get; set; } = null!;

    public DateTime GeneratedDate { get; set; }

    public string Data { get; set; } = null!;

    public Guid AccountId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Account Account { get; set; } = null!;
}
