using System;

namespace EVStationRental.Common.DTOs.VehicleDTOs
{
    public class ViewVehicleResponse
    {
        public Guid VehicleId { get; set; }
        public string SerialNumber { get; set; }
        public Guid ModelId { get; set; }
        public Guid? StationId { get; set; }
        public int? BatteryLevel { get; set; }
        public int? BatteryCapacity { get; set; }
        public int? Range { get; set; }
        public string? Color { get; set; }
        public DateOnly? LastMaintenance { get; set; }
        public string? Img { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Isactive { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
