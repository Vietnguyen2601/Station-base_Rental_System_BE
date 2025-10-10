using System;

namespace EVStationRental.Common.DTOs.StationDTOs
{
    public class UpdateStationRequestDTO
    {
        // Kh�ng cho ph�p ch?nh s?a StationId, ch? d�ng ?? x�c ??nh tr?m c?n c?p nh?t
        public string? Name { get; set; }
        public string? Address { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }
        public int? Capacity { get; set; }
    }
}
