using System;

namespace EVStationRental.Common.DTOs.DashboardDTOs
{
    /// <summary>
    /// DTO for Station Revenue Summary
    /// </summary>
    public class StationRevenueDTO
    {
        public Guid StationId { get; set; }
        public string StationName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal UsageRate { get; set; } // T? l? s? d?ng (%)
        public int TotalVehicles { get; set; }
        public int RentedVehicles { get; set; }
    }
}
