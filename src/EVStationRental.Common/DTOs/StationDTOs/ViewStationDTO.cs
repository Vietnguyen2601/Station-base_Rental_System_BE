using System;
using System.Collections.Generic;

namespace EVStationRental.Common.DTOs.StationDTOs
{
    /// <summary>
    /// DTO for viewing station with basic vehicle info
    /// </summary>
    public class ViewStationDTO
    {
        public Guid StationId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
        public int Capacity { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Basic vehicle count instead of full vehicle list
        public int TotalVehicles { get; set; }
        public int AvailableVehicles { get; set; }
        public int RentedVehicles { get; set; }
        public int MaintenanceVehicles { get; set; }
        public int ChargingVehicles { get; set; }
    }
}
