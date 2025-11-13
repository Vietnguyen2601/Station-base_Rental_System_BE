using System;
using System.Collections.Generic;

namespace EVStationRental.Common.DTOs.DashboardDTOs
{
    /// <summary>
    /// Response DTO for Vehicle Model Usage Dashboard
    /// </summary>
    public class VehicleModelUsageDashboardResponseDTO
    {
        public string FilterDescription { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public int TotalModels { get; set; }
        public int TotalVehicles { get; set; }
        public decimal OverallRentalRate { get; set; } // T? l? thuê trung bình
        public decimal OverallAvailabilityRate { get; set; } // T? l? tr?ng trung bình
        public List<VehicleModelUsageDTO> VehicleModels { get; set; } = new();
    }
}
