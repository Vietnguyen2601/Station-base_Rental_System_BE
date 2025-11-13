using EVStationRental.Common.DTOs.DashboardDTOs;
using EVStationRental.Services.InternalServices.IServices.IDashboardServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EVStationRental.APIServices.Controllers
{
    /// <summary>
    /// Dashboard Controller - Station Revenue Analytics
    /// Provides comprehensive revenue and usage analytics by station
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all dashboard endpoints
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Get station revenue summary with flexible filters
        /// Supports Month, Quarter, and Custom Date filters
        /// </summary>
        /// <param name="filter">Filter parameters (Month, Quarter, or CustomDate)</param>
        /// <returns>Revenue summary by station, sorted by revenue (high to low)</returns>
        /// <remarks>
        /// Examples:
        /// - Monthly: ?Month=10&amp;Year=2024
        /// - Quarterly: ?Quarter=4&amp;Year=2024
        /// - Custom: ?StartDate=2024-10-01&amp;EndDate=2024-10-31
        /// 
        /// If no filter provided, returns current month data
        /// </remarks>
        [HttpGet("station-revenue")]
        public async Task<IActionResult> GetStationRevenue([FromQuery] DashboardFilterDTO filter)
        {
            var result = await _dashboardService.GetStationRevenueAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get station revenue by month
        /// </summary>
        /// <param name="month">Month (1-12)</param>
        /// <param name="year">Year</param>
        [HttpGet("station-revenue/by-month")]
        public async Task<IActionResult> GetStationRevenueByMonth([FromQuery] int month, [FromQuery] int year)
        {
            if (month < 1 || month > 12)
            {
                return BadRequest(new { message = "Tháng ph?i t? 1 ??n 12" });
            }

            var result = await _dashboardService.GetDashboardByMonthAsync(month, year);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get station revenue by quarter
        /// </summary>
        /// <param name="quarter">Quarter (1-4)</param>
        /// <param name="year">Year</param>
        [HttpGet("station-revenue/by-quarter")]
        public async Task<IActionResult> GetStationRevenueByQuarter([FromQuery] int quarter, [FromQuery] int year)
        {
            if (quarter < 1 || quarter > 4)
            {
                return BadRequest(new { message = "Quý ph?i t? 1 ??n 4" });
            }

            var result = await _dashboardService.GetDashboardByQuarterAsync(quarter, year);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get station revenue by custom date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        [HttpGet("station-revenue/by-custom-date")]
        public async Task<IActionResult> GetStationRevenueByCustomDate(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var result = await _dashboardService.GetDashboardByCustomDateAsync(startDate, endDate);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get order count by station with filters
        /// Breaks down orders by status (Pending, Confirmed, Ongoing, Completed, Canceled)
        /// </summary>
        /// <param name="filter">Filter parameters</param>
        [HttpGet("station-orders")]
        public async Task<IActionResult> GetStationOrderCount([FromQuery] DashboardFilterDTO filter)
        {
            var result = await _dashboardService.GetStationOrderCountAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get order count by station - by month
        /// </summary>
        [HttpGet("station-orders/by-month")]
        public async Task<IActionResult> GetStationOrdersByMonth([FromQuery] int month, [FromQuery] int year)
        {
            if (month < 1 || month > 12)
            {
                return BadRequest(new { message = "Tháng ph?i t? 1 ??n 12" });
            }

            var filter = new DashboardFilterDTO { Month = month, Year = year };
            var result = await _dashboardService.GetStationOrderCountAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get order count by station - by quarter
        /// </summary>
        [HttpGet("station-orders/by-quarter")]
        public async Task<IActionResult> GetStationOrdersByQuarter([FromQuery] int quarter, [FromQuery] int year)
        {
            if (quarter < 1 || quarter > 4)
            {
                return BadRequest(new { message = "Quý ph?i t? 1 ??n 4" });
            }

            var filter = new DashboardFilterDTO { Quarter = quarter, Year = year };
            var result = await _dashboardService.GetStationOrderCountAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get order count by station - by custom date
        /// </summary>
        [HttpGet("station-orders/by-custom-date")]
        public async Task<IActionResult> GetStationOrdersByCustomDate(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var filter = new DashboardFilterDTO { StartDate = startDate, EndDate = endDate };
            var result = await _dashboardService.GetStationOrderCountAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get vehicle usage rate by station
        /// Shows current vehicle status distribution and usage metrics
        /// </summary>
        /// <param name="filter">Filter parameters (mainly for consistency, usage is real-time)</param>
        [HttpGet("station-usage")]
        public async Task<IActionResult> GetStationVehicleUsage([FromQuery] DashboardFilterDTO filter)
        {
            var result = await _dashboardService.GetStationVehicleUsageAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get current vehicle usage rate by station
        /// Real-time snapshot of vehicle availability
        /// </summary>
        [HttpGet("station-usage/current")]
        public async Task<IActionResult> GetCurrentStationUsage()
        {
            var filter = new DashboardFilterDTO(); // Empty filter for current snapshot
            var result = await _dashboardService.GetStationVehicleUsageAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get vehicle usage statistics by vehicle model
        /// Shows rental rate and availability rate for each vehicle model
        /// Supports Month, Quarter, and Custom Date filters
        /// </summary>
        /// <param name="filter">Filter parameters (Month, Quarter, or CustomDate)</param>
        /// <remarks>
        /// Returns statistics for all vehicle models including:
        /// - Total vehicles per model
        /// - Rental rate (percentage of rented vehicles)
        /// - Availability rate (percentage of available vehicles)
        /// - Maintenance and charging rates
        /// 
        /// Examples:
        /// - Monthly: ?Month=10&amp;Year=2024
        /// - Quarterly: ?Quarter=4&amp;Year=2024
        /// - Custom: ?StartDate=2024-10-01&amp;EndDate=2024-10-31
        /// 
        /// If no filter provided, shows current real-time status
        /// Sorted by rental rate (highest first)
        /// </remarks>
        [HttpGet("vehicle-model-usage")]
        public async Task<IActionResult> GetVehicleModelUsage([FromQuery] DashboardFilterDTO filter)
        {
            var result = await _dashboardService.GetVehicleModelUsageAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get vehicle model usage by month
        /// </summary>
        [HttpGet("vehicle-model-usage/by-month")]
        public async Task<IActionResult> GetVehicleModelUsageByMonth([FromQuery] int month, [FromQuery] int year)
        {
            if (month < 1 || month > 12)
            {
                return BadRequest(new { message = "Tháng ph?i t? 1 ??n 12" });
            }

            var filter = new DashboardFilterDTO { Month = month, Year = year };
            var result = await _dashboardService.GetVehicleModelUsageAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get vehicle model usage by quarter
        /// </summary>
        [HttpGet("vehicle-model-usage/by-quarter")]
        public async Task<IActionResult> GetVehicleModelUsageByQuarter([FromQuery] int quarter, [FromQuery] int year)
        {
            if (quarter < 1 || quarter > 4)
            {
                return BadRequest(new { message = "Quý ph?i t? 1 ??n 4" });
            }

            var filter = new DashboardFilterDTO { Quarter = quarter, Year = year };
            var result = await _dashboardService.GetVehicleModelUsageAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get vehicle model usage by custom date range
        /// </summary>
        [HttpGet("vehicle-model-usage/by-custom-date")]
        public async Task<IActionResult> GetVehicleModelUsageByCustomDate(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var filter = new DashboardFilterDTO { StartDate = startDate, EndDate = endDate };
            var result = await _dashboardService.GetVehicleModelUsageAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get current vehicle model usage (real-time)
        /// </summary>
        [HttpGet("vehicle-model-usage/current")]
        public async Task<IActionResult> GetCurrentVehicleModelUsage()
        {
            var filter = new DashboardFilterDTO(); // Empty filter for current snapshot
            var result = await _dashboardService.GetVehicleModelUsageAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get top stations by revenue
        /// Supports Month, Quarter, and Custom Date filters
        /// </summary>
        /// <param name="filter">Filter parameters</param>
        /// <param name="topCount">Number of top stations to return (default: 10)</param>
        /// <remarks>
        /// Returns top stations ranked by total revenue (highest to lowest).
        /// 
        /// Examples:
        /// - Monthly: ?Month=10&amp;Year=2024&amp;topCount=5
        /// - Quarterly: ?Quarter=4&amp;Year=2024&amp;topCount=10
        /// - Custom: ?StartDate=2024-10-01&amp;EndDate=2024-10-31&amp;topCount=3
        /// 
        /// Default: Returns top 10 stations for current month
        /// </remarks>
        [HttpGet("top-stations")]
        public async Task<IActionResult> GetTopStationsByRevenue(
            [FromQuery] DashboardFilterDTO filter,
            [FromQuery] int topCount = 10)
        {
            if (topCount <= 0)
            {
                return BadRequest(new { message = "topCount ph?i l?n h?n 0" });
            }

            if (topCount > 50)
            {
                return BadRequest(new { message = "topCount không ???c v??t quá 50" });
            }

            var result = await _dashboardService.GetTopStationsByRevenueAsync(filter, topCount);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get top stations by revenue - By Month
        /// </summary>
        [HttpGet("top-stations/by-month")]
        public async Task<IActionResult> GetTopStationsByMonth(
            [FromQuery] int month,
            [FromQuery] int year,
            [FromQuery] int topCount = 10)
        {
            if (month < 1 || month > 12)
            {
                return BadRequest(new { message = "Tháng ph?i t? 1 ??n 12" });
            }

            if (topCount <= 0 || topCount > 50)
            {
                return BadRequest(new { message = "topCount ph?i t? 1 ??n 50" });
            }

            var filter = new DashboardFilterDTO { Month = month, Year = year };
            var result = await _dashboardService.GetTopStationsByRevenueAsync(filter, topCount);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get top stations by revenue - By Quarter
        /// </summary>
        [HttpGet("top-stations/by-quarter")]
        public async Task<IActionResult> GetTopStationsByQuarter(
            [FromQuery] int quarter,
            [FromQuery] int year,
            [FromQuery] int topCount = 10)
        {
            if (quarter < 1 || quarter > 4)
            {
                return BadRequest(new { message = "Quý ph?i t? 1 ??n 4" });
            }

            if (topCount <= 0 || topCount > 50)
            {
                return BadRequest(new { message = "topCount ph?i t? 1 ??n 50" });
            }

            var filter = new DashboardFilterDTO { Quarter = quarter, Year = year };
            var result = await _dashboardService.GetTopStationsByRevenueAsync(filter, topCount);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get top stations by revenue - By Custom Date Range
        /// </summary>
        [HttpGet("top-stations/by-custom-date")]
        public async Task<IActionResult> GetTopStationsByCustomDate(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int topCount = 10)
        {
            if (topCount <= 0 || topCount > 50)
            {
                return BadRequest(new { message = "topCount ph?i t? 1 ??n 50" });
            }

            var filter = new DashboardFilterDTO { StartDate = startDate, EndDate = endDate };
            var result = await _dashboardService.GetTopStationsByRevenueAsync(filter, topCount);
            return StatusCode(result.StatusCode, result);
        }
    }
}
