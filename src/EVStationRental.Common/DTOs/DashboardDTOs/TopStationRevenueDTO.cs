using System;

namespace EVStationRental.Common.DTOs.DashboardDTOs
{
    /// <summary>
    /// DTO for Top Station Revenue Ranking
    /// </summary>
    public class TopStationRevenueDTO
    {
        public int Rank { get; set; }
        public Guid StationId { get; set; }
        public string StationName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalVehicles { get; set; }
        public int RentedVehicles { get; set; }
        public decimal UsageRate { get; set; }
    }
}
