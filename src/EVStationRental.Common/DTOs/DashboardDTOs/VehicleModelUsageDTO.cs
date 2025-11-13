using System;

namespace EVStationRental.Common.DTOs.DashboardDTOs
{
    /// <summary>
    /// DTO for Vehicle Model Usage Statistics
    /// </summary>
    public class VehicleModelUsageDTO
    {
        public Guid VehicleModelId { get; set; }
        public string ModelName { get; set; } = null!;
        public string Manufacturer { get; set; } = null!;
        public string VehicleType { get; set; } = null!;
        public decimal PricePerHour { get; set; }
        
        public int TotalVehicles { get; set; }
        public int AvailableVehicles { get; set; }
        public int RentedVehicles { get; set; }
        public int MaintenanceVehicles { get; set; }
        public int ChargingVehicles { get; set; }
        
        public decimal RentalRate { get; set; } // T? l? thuê (%)
        public decimal AvailabilityRate { get; set; } // T? l? tr?ng (%)
        public decimal MaintenanceRate { get; set; } // T? l? b?o trì (%)
        public decimal ChargingRate { get; set; } // T? l? ?ang s?c (%)
    }
}
