using System.Collections.Generic;
using System.Linq;
using EVStationRental.Common.DTOs.DashboardDTOs;
using EVStationRental.Repositories.Models;

namespace EVStationRental.Repositories.Mapper
{
    public static class DashboardMapper
    {
        /// <summary>
        /// Map Station with revenue data to StationRevenueDTO
        /// </summary>
        public static StationRevenueDTO ToStationRevenueDTO(
            this Station station,
            decimal totalRevenue,
            int totalOrders,
            int completedOrders,
            int totalVehicles,
            int rentedVehicles)
        {
            var usageRate = totalVehicles > 0 
                ? (decimal)rentedVehicles / totalVehicles * 100 
                : 0;

            return new StationRevenueDTO
            {
                StationId = station.StationId,
                StationName = station.Name,
                Address = station.Address,
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                CompletedOrders = completedOrders,
                UsageRate = decimal.Round(usageRate, 2),
                TotalVehicles = totalVehicles,
                RentedVehicles = rentedVehicles
            };
        }

        /// <summary>
        /// Map list of station revenues to dashboard response
        /// </summary>
        public static StationRevenueDashboardResponseDTO ToDashboardResponse(
            this List<StationRevenueDTO> stationRevenues,
            string filterType,
            string filterDescription,
            System.DateTime? startDate = null,
            System.DateTime? endDate = null)
        {
            return new StationRevenueDashboardResponseDTO
            {
                FilterType = filterType,
                FilterDescription = filterDescription,
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = stationRevenues.Sum(s => s.TotalRevenue),
                TotalOrders = stationRevenues.Sum(s => s.TotalOrders),
                StationRevenues = stationRevenues.OrderByDescending(s => s.TotalRevenue).ToList()
            };
        }
    }
}
