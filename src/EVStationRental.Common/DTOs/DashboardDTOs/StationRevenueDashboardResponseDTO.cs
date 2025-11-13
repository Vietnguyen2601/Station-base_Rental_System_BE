using System;
using System.Collections.Generic;

namespace EVStationRental.Common.DTOs.DashboardDTOs
{
    /// <summary>
    /// Response DTO for Station Revenue Dashboard
    /// </summary>
    public class StationRevenueDashboardResponseDTO
    {
        public string FilterType { get; set; } = null!; // "Month", "Quarter", "CustomDate"
        public string FilterDescription { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public List<StationRevenueDTO> StationRevenues { get; set; } = new();
    }
}
