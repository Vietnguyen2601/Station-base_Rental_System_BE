using System;

namespace EVStationRental.Common.DTOs.VehicleDTOs
{
    public class UpdateVehicleRequestDTO
    {
        // Không cho phép c?p nh?t VehicleId, không hi?n field này ? ph?n request
        public Guid? StationId { get; set; }
        public Guid? ModelId { get; set; }
        public int? BatteryLevel { get; set; }
        public decimal? LocationLat { get; set; }
        public decimal? LocationLong { get; set; }
        public DateOnly? LastMaintenance { get; set; }
    }
}
