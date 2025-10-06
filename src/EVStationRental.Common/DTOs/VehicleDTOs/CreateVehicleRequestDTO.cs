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
        public decimal? LocationLat { get; set; }
        public decimal? LocationLong { get; set; }
        public DateOnly? LastMaintenance { get; set; }
    }
}
