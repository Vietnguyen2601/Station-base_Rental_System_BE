using System;

namespace EVStationRental.Common.DTOs.StationDTOs
{
    public class UpdateStationRequestDTO
    {
        // Không cho phép ch?nh s?a StationId, ch? dùng ?? xác ??nh tr?m c?n c?p nh?t
        public string? Name { get; set; }
        public string? Address { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }
        public int? Capacity { get; set; }
    }
}
