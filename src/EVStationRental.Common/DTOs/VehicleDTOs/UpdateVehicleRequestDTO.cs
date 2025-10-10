using System;

namespace EVStationRental.Common.DTOs.VehicleDTOs
{
    public class UpdateVehicleRequestDTO
    {
        // Kh�ng cho ph�p c?p nh?t VehicleId, kh�ng hi?n field n�y ? ph?n request
        public Guid? StationId { get; set; }
        public Guid? ModelId { get; set; }
        public int? BatteryLevel { get; set; }
        public decimal? LocationLat { get; set; }
        public decimal? LocationLong { get; set; }
        public DateOnly? LastMaintenance { get; set; }
    }
}
