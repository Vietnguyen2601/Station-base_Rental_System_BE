using System;
using System.Collections.Generic;

namespace EVStationRental.Common.DTOs.DashboardDTOs
{
    /// <summary>
    /// Response DTO for Top Stations Ranking
    /// </summary>
    public class TopStationsDashboardResponseDTO
    {
        public string FilterDescription { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalStations { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TopCount { get; set; }
        public List<TopStationRevenueDTO> TopStations { get; set; } = new();
        //Demo
    }
}
