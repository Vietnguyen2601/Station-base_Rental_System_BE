using System;

namespace EVStationRental.Common.DTOs.VehicleDTOs
{
    public class UpdateVehicleRequestDTO
    {
        public Guid? StationId { get; set; }
        public Guid? ModelId { get; set; }
        public int? BatteryLevel { get; set; }
        public int? BatteryCapacity { get; set; }
        public int? Range { get; set; }
        public string? Color { get; set; }
        public DateOnly? LastMaintenance { get; set; }
        public string? Img { get; set; }
        public bool? Isactive { get; set; }
    }
}
