using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Common.DTOs.VehicleDTOs
{
    public class CreateVehicleRequestDTO
    {
        public string SerialNumber { get; set; }
        public Guid ModelId { get; set; }
        public Guid? StationId { get; set; }
        public int? BatteryLevel { get; set; }
        public int? BatteryCapacity { get; set; }
        public int? Range { get; set; }
        public string? Color { get; set; }
        public DateOnly? LastMaintenance { get; set; }
        public string? Img { get; set; }
        public bool? Isactive { get; set; }
        public decimal? LocationLat { get; set; }
        public decimal? LocationLong { get; set; }
    }
}
