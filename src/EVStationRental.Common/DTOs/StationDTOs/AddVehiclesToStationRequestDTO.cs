using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EVStationRental.Common.DTOs.StationDTOs
{
    public class AddVehiclesToStationRequestDTO
    {
        [Required]
        public Guid StationId { get; set; }
        [Required]
        public List<Guid> VehicleIds { get; set; } = new List<Guid>();
    }
}
