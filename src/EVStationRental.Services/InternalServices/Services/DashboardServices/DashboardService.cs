using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVStationRental.Common.DTOs.DashboardDTOs;
using EVStationRental.Common.Enums.EnumModel;
using EVStationRental.Common.Enums.ServiceResultEnum;
using EVStationRental.Repositories.Mapper;
using EVStationRental.Repositories.UnitOfWork;
using EVStationRental.Services.Base;
using EVStationRental.Services.InternalServices.IServices.IDashboardServices;
using Microsoft.Extensions.Logging;

namespace EVStationRental.Services.InternalServices.Services.DashboardServices
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(IUnitOfWork unitOfWork, ILogger<DashboardService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get station revenue with filters
        /// </summary>
        public async Task<IServiceResult> GetStationRevenueAsync(DashboardFilterDTO filter)
        {
            try
            {
                var dateRange = GetDateRangeFromFilter(filter);
                var stations = await _unitOfWork.StationRepository.GetAllStationsAsync();
                var allOrders = await _unitOfWork.OrderRepository.GetAllOrdersAsync();

                var stationRevenues = new List<StationRevenueDTO>();

                foreach (var station in stations)
                {
                    // Get orders for this station in date range
                    var stationOrders = allOrders
                        .Where(o => o.Vehicle != null && o.Vehicle.StationId == station.StationId)
                        .Where(o => o.CreatedAt >= dateRange.StartDate && o.CreatedAt <= dateRange.EndDate)
                        .ToList();

                    var completedOrders = stationOrders.Where(o => o.Status == OrderStatus.COMPLETED).ToList();
                    var totalRevenue = completedOrders.Sum(o => o.TotalPrice);

                    // Get vehicle count for this station
                    var totalVehicles = station.Vehicles.Count(v => v.Isactive);
                    var rentedVehicles = station.Vehicles.Count(v => v.Status == VehicleStatus.RENTED);

                    var dto = station.ToStationRevenueDTO(
                        totalRevenue,
                        stationOrders.Count,
                        completedOrders.Count,
                        totalVehicles,
                        rentedVehicles
                    );

                    stationRevenues.Add(dto);
                }

                // Sort by revenue descending
                var sortedRevenues = stationRevenues
                    .OrderByDescending(s => s.TotalRevenue)
                    .ToList();

                var response = sortedRevenues.ToDashboardResponse(
                    dateRange.FilterType,
                    dateRange.Description,
                    dateRange.StartDate,
                    dateRange.EndDate
                );

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = "L?y d? li?u doanh thu theo tr?m thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting station revenue");
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"L?i khi l?y d? li?u doanh thu: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Get order count by station
        /// </summary>
        public async Task<IServiceResult> GetStationOrderCountAsync(DashboardFilterDTO filter)
        {
            try
            {
                var dateRange = GetDateRangeFromFilter(filter);
                var stations = await _unitOfWork.StationRepository.GetAllStationsAsync();
                var allOrders = await _unitOfWork.OrderRepository.GetAllOrdersAsync();

                var result = new List<object>();

                foreach (var station in stations)
                {
                    var stationOrders = allOrders
                        .Where(o => o.Vehicle != null && o.Vehicle.StationId == station.StationId)
                        .Where(o => o.CreatedAt >= dateRange.StartDate && o.CreatedAt <= dateRange.EndDate)
                        .ToList();

                    result.Add(new
                    {
                        StationId = station.StationId,
                        StationName = station.Name,
                        Address = station.Address,
                        TotalOrders = stationOrders.Count,
                        PendingOrders = stationOrders.Count(o => o.Status == OrderStatus.PENDING),
                        ConfirmedOrders = stationOrders.Count(o => o.Status == OrderStatus.CONFIRMED),
                        OngoingOrders = stationOrders.Count(o => o.Status == OrderStatus.ONGOING),
                        CompletedOrders = stationOrders.Count(o => o.Status == OrderStatus.COMPLETED),
                        CanceledOrders = stationOrders.Count(o => o.Status == OrderStatus.CANCELED)
                    });
                }

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = "L?y s? l??ng ??n hàng theo tr?m thành công",
                    Data = new
                    {
                        FilterType = dateRange.FilterType,
                        FilterDescription = dateRange.Description,
                        StartDate = dateRange.StartDate,
                        EndDate = dateRange.EndDate,
                        Stations = result.OrderByDescending(s => ((dynamic)s).TotalOrders).ToList()
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting station order count");
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"L?i khi l?y s? l??ng ??n hàng: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Get vehicle usage rate by station
        /// </summary>
        public async Task<IServiceResult> GetStationVehicleUsageAsync(DashboardFilterDTO filter)
        {
            try
            {
                var dateRange = GetDateRangeFromFilter(filter);
                var stations = await _unitOfWork.StationRepository.GetAllStationsAsync();

                var result = new List<object>();

                foreach (var station in stations)
                {
                    var totalVehicles = station.Vehicles.Count(v => v.Isactive);
                    var availableVehicles = station.Vehicles.Count(v => v.Status == VehicleStatus.AVAILABLE);
                    var rentedVehicles = station.Vehicles.Count(v => v.Status == VehicleStatus.RENTED);
                    var maintenanceVehicles = station.Vehicles.Count(v => v.Status == VehicleStatus.MAINTENANCE);
                    var chargingVehicles = station.Vehicles.Count(v => v.Status == VehicleStatus.CHARGING);

                    // Fix division by zero error
                    var usageRate = totalVehicles > 0 
                        ? decimal.Round((decimal)rentedVehicles / totalVehicles * 100, 2)
                        : 0;

                    var availabilityRate = totalVehicles > 0
                        ? decimal.Round((decimal)availableVehicles / totalVehicles * 100, 2)
                        : 0;

                    result.Add(new
                    {
                        StationId = station.StationId,
                        StationName = station.Name,
                        Address = station.Address,
                        TotalVehicles = totalVehicles,
                        AvailableVehicles = availableVehicles,
                        RentedVehicles = rentedVehicles,
                        MaintenanceVehicles = maintenanceVehicles,
                        ChargingVehicles = station.Vehicles.Count(v => v.Status == VehicleStatus.CHARGING),
                        UsageRate = usageRate,
                        AvailabilityRate = availabilityRate
                    });
                }

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = "L?y t? l? s? d?ng xe theo tr?m thành công",
                    Data = new
                    {
                        FilterType = dateRange.FilterType,
                        FilterDescription = dateRange.Description,
                        Timestamp = DateTime.Now,
                        Stations = result.OrderByDescending(s => ((dynamic)s).UsageRate).ToList()
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting station vehicle usage");
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"L?i khi l?y t? l? s? d?ng xe: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Get dashboard by month
        /// </summary>
        public async Task<IServiceResult> GetDashboardByMonthAsync(int month, int year)
        {
            var filter = new DashboardFilterDTO
            {
                Month = month,
                Year = year
            };

            return await GetStationRevenueAsync(filter);
        }

        /// <summary>
        /// Get dashboard by quarter
        /// </summary>
        public async Task<IServiceResult> GetDashboardByQuarterAsync(int quarter, int year)
        {
            var filter = new DashboardFilterDTO
            {
                Quarter = quarter,
                Year = year
            };

            return await GetStationRevenueAsync(filter);
        }

        /// <summary>
        /// Get dashboard by custom date range
        /// </summary>
        public async Task<IServiceResult> GetDashboardByCustomDateAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_VALIDATION_CODE,
                    Message = "Ngày b?t ??u không ???c l?n h?n ngày k?t thúc"
                };
            }

            var filter = new DashboardFilterDTO
            {
                StartDate = startDate,
                EndDate = endDate
            };

            return await GetStationRevenueAsync(filter);
        }

        /// <summary>
        /// Get vehicle usage statistics by vehicle model
        /// Shows usage rate based on rental history in the filtered period
        /// </summary>
        public async Task<IServiceResult> GetVehicleModelUsageAsync(DashboardFilterDTO filter)
        {
            try
            {
                var dateRange = GetDateRangeFromFilter(filter);
                var vehicleModels = await _unitOfWork.VehicleModelRepository.GetAllVehicleModelsAsync();
                var allVehicles = await _unitOfWork.VehicleRepository.GetAllVehiclesAsync();
                var allOrders = await _unitOfWork.OrderRepository.GetAllOrdersAsync();

                var modelUsageList = new List<VehicleModelUsageDTO>();
                int totalVehiclesCount = 0;
                int totalRentedCount = 0;
                int totalAvailableCount = 0;

                foreach (var model in vehicleModels)
                {
                    // Get all active vehicles for this model
                    var modelVehicles = allVehicles
                        .Where(v => v.ModelId == model.VehicleModelId && v.Isactive)
                        .ToList();

                    var totalVehicles = modelVehicles.Count;
                    
                    // Current status (real-time)
                    var availableVehicles = modelVehicles.Count(v => v.Status == VehicleStatus.AVAILABLE);
                    var rentedVehicles = modelVehicles.Count(v => v.Status == VehicleStatus.RENTED);
                    var maintenanceVehicles = modelVehicles.Count(v => v.Status == VehicleStatus.MAINTENANCE);
                    var chargingVehicles = modelVehicles.Count(v => v.Status == VehicleStatus.CHARGING);

                    // Historical usage in filtered period
                    var modelOrders = allOrders
                        .Where(o => o.Vehicle != null && o.Vehicle.ModelId == model.VehicleModelId)
                        .Where(o => o.CreatedAt >= dateRange.StartDate && o.CreatedAt <= dateRange.EndDate)
                        .ToList();

                    var totalOrdersInPeriod = modelOrders.Count;
                    var completedOrdersInPeriod = modelOrders.Count(o => o.Status == OrderStatus.COMPLETED);

                    // Calculate rates (fix division by zero)
                    var rentalRate = totalVehicles > 0
                        ? decimal.Round((decimal)rentedVehicles / totalVehicles * 100, 2)
                        : 0;

                    var availabilityRate = totalVehicles > 0
                        ? decimal.Round((decimal)availableVehicles / totalVehicles * 100, 2)
                        : 0;

                    var maintenanceRate = totalVehicles > 0
                        ? decimal.Round((decimal)maintenanceVehicles / totalVehicles * 100, 2)
                        : 0;

                    var chargingRate = totalVehicles > 0
                        ? decimal.Round((decimal)chargingVehicles / totalVehicles * 100, 2)
                        : 0;

                    var dto = new VehicleModelUsageDTO
                    {
                        VehicleModelId = model.VehicleModelId,
                        ModelName = model.Name,
                        Manufacturer = model.Manufacturer,
                        VehicleType = model.Type?.TypeName ?? "Unknown",
                        PricePerHour = model.PricePerHour,
                        TotalVehicles = totalVehicles,
                        AvailableVehicles = availableVehicles,
                        RentedVehicles = rentedVehicles,
                        MaintenanceVehicles = maintenanceVehicles,
                        ChargingVehicles = chargingVehicles,
                        RentalRate = rentalRate,
                        AvailabilityRate = availabilityRate,
                        MaintenanceRate = maintenanceRate,
                        ChargingRate = chargingRate
                    };

                    modelUsageList.Add(dto);

                    // Accumulate totals
                    totalVehiclesCount += totalVehicles;
                    totalRentedCount += rentedVehicles;
                    totalAvailableCount += availableVehicles;
                }

                // Calculate overall rates
                var overallRentalRate = totalVehiclesCount > 0
                    ? decimal.Round((decimal)totalRentedCount / totalVehiclesCount * 100, 2)
                    : 0;

                var overallAvailabilityRate = totalVehiclesCount > 0
                    ? decimal.Round((decimal)totalAvailableCount / totalVehiclesCount * 100, 2)
                    : 0;

                // Sort by rental rate descending (most rented first)
                var sortedModels = modelUsageList
                    .OrderByDescending(m => m.RentalRate)
                    .ToList();

                var response = new VehicleModelUsageDashboardResponseDTO
                {
                    FilterDescription = dateRange.Description,
                    Timestamp = DateTime.Now,
                    TotalModels = modelUsageList.Count,
                    TotalVehicles = totalVehiclesCount,
                    OverallRentalRate = overallRentalRate,
                    OverallAvailabilityRate = overallAvailabilityRate,
                    VehicleModels = sortedModels
                };

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = "L?y th?ng kê t? l? s? d?ng theo lo?i xe thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicle model usage");
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"L?i khi l?y th?ng kê t? l? s? d?ng theo lo?i xe: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Get top stations by revenue (highest to lowest)
        /// </summary>
        public async Task<IServiceResult> GetTopStationsByRevenueAsync(DashboardFilterDTO filter, int topCount = 10)
        {
            try
            {
                var dateRange = GetDateRangeFromFilter(filter);
                var stations = await _unitOfWork.StationRepository.GetAllStationsAsync();
                var allOrders = await _unitOfWork.OrderRepository.GetAllOrdersAsync();

                var stationRevenueList = new List<TopStationRevenueDTO>();
                decimal totalSystemRevenue = 0;

                foreach (var station in stations)
                {
                    // Get orders for this station in date range
                    var stationOrders = allOrders
                        .Where(o => o.Vehicle != null && o.Vehicle.StationId == station.StationId)
                        .Where(o => o.CreatedAt >= dateRange.StartDate && o.CreatedAt <= dateRange.EndDate)
                        .ToList();

                    var completedOrders = stationOrders.Where(o => o.Status == OrderStatus.COMPLETED).ToList();
                    var totalRevenue = completedOrders.Sum(o => o.TotalPrice);

                    // Get vehicle statistics
                    var totalVehicles = station.Vehicles.Count(v => v.Isactive);
                    var rentedVehicles = station.Vehicles.Count(v => v.Status == VehicleStatus.RENTED);

                    var usageRate = totalVehicles > 0
                        ? decimal.Round((decimal)rentedVehicles / totalVehicles * 100, 2)
                        : 0;

                    var averageOrderValue = completedOrders.Count > 0
                        ? decimal.Round(totalRevenue / completedOrders.Count, 2)
                        : 0;

                    stationRevenueList.Add(new TopStationRevenueDTO
                    {
                        StationId = station.StationId,
                        StationName = station.Name,
                        Address = station.Address,
                        TotalRevenue = totalRevenue,
                        TotalOrders = stationOrders.Count,
                        CompletedOrders = completedOrders.Count,
                        AverageOrderValue = averageOrderValue,
                        TotalVehicles = totalVehicles,
                        RentedVehicles = rentedVehicles,
                        UsageRate = usageRate
                    });

                    totalSystemRevenue += totalRevenue;
                }

                // Sort by revenue descending and take top N
                var topStations = stationRevenueList
                    .OrderByDescending(s => s.TotalRevenue)
                    .Take(topCount)
                    .Select((station, index) => 
                    {
                        station.Rank = index + 1;
                        return station;
                    })
                    .ToList();

                var response = new TopStationsDashboardResponseDTO
                {
                    FilterDescription = dateRange.Description,
                    StartDate = dateRange.StartDate,
                    EndDate = dateRange.EndDate,
                    TotalStations = stationRevenueList.Count,
                    TotalRevenue = totalSystemRevenue,
                    TopCount = topStations.Count,
                    TopStations = topStations
                };

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = $"L?y top {topCount} tr?m có doanh thu cao nh?t thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top stations by revenue");
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"L?i khi l?y top tr?m theo doanh thu: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Helper method to get date range from filter
        /// </summary>
        private (DateTime StartDate, DateTime EndDate, string FilterType, string Description) GetDateRangeFromFilter(DashboardFilterDTO filter)
        {
            DateTime startDate, endDate;
            string filterType, description;

            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                // Custom date range
                startDate = filter.StartDate.Value;
                endDate = filter.EndDate.Value.Date.AddDays(1).AddSeconds(-1);
                filterType = "CustomDate";
                description = $"T? {startDate:dd/MM/yyyy} ??n {filter.EndDate.Value:dd/MM/yyyy}";
            }
            else if (filter.Quarter.HasValue)
            {
                // Quarter filter
                var year = filter.Year ?? DateTime.Now.Year;
                var quarter = filter.Quarter.Value;

                startDate = new DateTime(year, (quarter - 1) * 3 + 1, 1);
                endDate = startDate.AddMonths(3).AddSeconds(-1);
                filterType = "Quarter";
                description = $"Quý {quarter} n?m {year}";
            }
            else if (filter.Month.HasValue)
            {
                // Month filter
                var year = filter.Year ?? DateTime.Now.Year;
                var month = filter.Month.Value;

                startDate = new DateTime(year, month, 1);
                endDate = startDate.AddMonths(1).AddSeconds(-1);
                filterType = "Month";
                description = $"Tháng {month} n?m {year}";
            }
            else
            {
                // Default: current month
                var now = DateTime.Now;
                startDate = new DateTime(now.Year, now.Month, 1);
                endDate = startDate.AddMonths(1).AddSeconds(-1);
                filterType = "Month";
                description = $"Tháng {now.Month} n?m {now.Year} (m?c ??nh)";
            }

            return (startDate, endDate, filterType, description);
        }
    }
}
