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
        public decimal? LocationLat { get; set; }
        public decimal? LocationLong { get; set; }
        public DateOnly? LastMaintenance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
