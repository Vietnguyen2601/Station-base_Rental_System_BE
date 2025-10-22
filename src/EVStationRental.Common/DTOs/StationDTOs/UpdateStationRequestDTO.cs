using System;

namespace EVStationRental.Common.DTOs.StationDTOs
{
    public class UpdateStationRequestDTO
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }
        public int? Capacity { get; set; }
        public string? ImageUrl { get; set; }
        public bool? Isactive { get; set; }
    }
}
