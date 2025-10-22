using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class Feedback
{
    public Guid FeedbackId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid OrderId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime FeedbackDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Isactive { get; set; }

    public virtual Account Customer { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
