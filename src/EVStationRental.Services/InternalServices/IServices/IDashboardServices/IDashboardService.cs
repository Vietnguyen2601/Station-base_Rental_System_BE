using System;
using System.Threading.Tasks;
using EVStationRental.Common.DTOs.DashboardDTOs;
using EVStationRental.Services.Base;

namespace EVStationRental.Services.InternalServices.IServices.IDashboardServices
{
    public interface IDashboardService
    {
        /// <summary>
        /// Get total revenue by station with filters
        /// </summary>
        Task<IServiceResult> GetStationRevenueAsync(DashboardFilterDTO filter);

        /// <summary>
        /// Get order count by station with filters
        /// </summary>
        Task<IServiceResult> GetStationOrderCountAsync(DashboardFilterDTO filter);

        /// <summary>
        /// Get vehicle usage rate by station with filters
        /// </summary>
        Task<IServiceResult> GetStationVehicleUsageAsync(DashboardFilterDTO filter);

        /// <summary>
        /// Get comprehensive dashboard by month
        /// </summary>
        Task<IServiceResult> GetDashboardByMonthAsync(int month, int year);

        /// <summary>
        /// Get comprehensive dashboard by quarter
        /// </summary>
        Task<IServiceResult> GetDashboardByQuarterAsync(int quarter, int year);

        /// <summary>
        /// Get comprehensive dashboard by custom date range
        /// </summary>
        Task<IServiceResult> GetDashboardByCustomDateAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get vehicle usage statistics by vehicle model
        /// Shows rental rate and availability rate for each vehicle model
        /// </summary>
        Task<IServiceResult> GetVehicleModelUsageAsync(DashboardFilterDTO filter);

        /// <summary>
        /// Get top stations by revenue (highest to lowest)
        /// </summary>
        Task<IServiceResult> GetTopStationsByRevenueAsync(DashboardFilterDTO filter, int topCount = 10);
    }
}
