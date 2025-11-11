using EVStationRental.Common.DTOs.StationDTOs;
using EVStationRental.Common.Enums.ServiceResultEnum;
using EVStationRental.Repositories.Mapper;
using EVStationRental.Repositories.UnitOfWork;
using EVStationRental.Services.Base;
using EVStationRental.Services.InternalServices.IServices.IStationServices;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace EVStationRental.Services.InternalServices.Services.StationServices
{
    public class StationService : IStationService
    {
        private readonly IUnitOfWork unitOfWork;

        public StationService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IServiceResult> CreateStationAsync(CreateStationRequestDTO dto)
        {
            // Ki?m tra Name không b? trùng
            var existingStation = (await unitOfWork.StationRepository.GetAllStationsAsync())
                .FirstOrDefault(s => s.Name == dto.Name);
            if (existingStation != null)
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_VALIDATION_CODE,
                    Message = "Tên tr?m ?ã t?n t?i"
                };
            }

            var station = dto.ToStation();
            var result = await unitOfWork.StationRepository.CreateStationAsync(station);
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_CREATE_CODE,
                Message = Const.SUCCESS_CREATE_MSG,
                Data = result
            };
        }

        public async Task<IServiceResult> GetAllStationsAsync()
        {
            var stations = await unitOfWork.StationRepository.GetAllStationsAsync();
            if (stations == null || stations.Count == 0)
            {
                return new ServiceResult
                {
                    StatusCode = Const.WARNING_NO_DATA_CODE,
                    Message = Const.WARNING_NO_DATA_MSG
                };
            }
            
            // Map to DTO to prevent circular reference
            var stationDTOs = stations.Select(s => s.ToViewStationDTO()).ToList();
            
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_READ_CODE,
                Message = Const.SUCCESS_READ_MSG,
                Data = stationDTOs
            };
        }

        public async Task<IServiceResult> GetVehiclesByStationIdAsync(Guid stationId)
        {
            var vehicles = await unitOfWork.StationRepository.GetVehiclesByStationIdAsync(stationId);
            if (vehicles == null || vehicles.Count == 0)
            {
                return new ServiceResult
                {
                    StatusCode = Const.WARNING_NO_DATA_CODE,
                    Message = "Không có xe nào trong tr?m này"
                };
            }
            var vehicleDTOs = vehicles.Select(v => v.ToViewVehicleDTO()).ToList();
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_READ_CODE,
                Message = Const.SUCCESS_READ_MSG,
                Data = vehicleDTOs
            };
        }

        public async Task<IServiceResult> AddVehiclesToStationAsync(AddVehiclesToStationRequestDTO dto)
        {
            var station = await unitOfWork.StationRepository.GetStationByIdAsync(dto.StationId);
            if (station == null)
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_VALIDATION_CODE,
                    Message = "StationId không h?p l?"
                };
            }
            if (dto.VehicleIds == null || dto.VehicleIds.Count == 0)
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_VALIDATION_CODE,
                    Message = "Danh sách xe không h?p l?"
                };
            }
            var success = await unitOfWork.StationRepository.AddVehiclesToStationAsync(dto.StationId, dto.VehicleIds);
            if (!success)
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = "Không th? thêm xe vào station"
                };
            }
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_CREATE_CODE,
                Message = "Thêm xe vào station thành công"
            };
        }

        public async Task<IServiceResult> UpdateStationAsync(Guid stationId, UpdateStationRequestDTO dto)
        {
            var station = await unitOfWork.StationRepository.GetStationByIdAsync(stationId);
            if (station == null)
                return new ServiceResult { StatusCode = Const.WARNING_NO_DATA_CODE, Message = "Không tìm th?y tr?m" };

            // Không cho ch?nh s?a StationId
            dto.MapToStation(station);

            var updated = await unitOfWork.StationRepository.UpdateStationAsync(station);
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_UPDATE_CODE,
                Message = Const.SUCCESS_UPDATE_MSG,
                Data = updated
            };
        }

        public async Task<IServiceResult> SoftDeleteStationAsync(Guid stationId)
        {
            var success = await unitOfWork.StationRepository.SoftDeleteStationAsync(stationId);
            if (!success)
                return new ServiceResult { StatusCode = Const.WARNING_NO_DATA_CODE, Message = "Không tìm th?y tr?m ho?c ?ã b? xóa" };
            return new ServiceResult { StatusCode = Const.SUCCESS_UPDATE_CODE, Message = "Xóa m?m tr?m thành công" };
        }

        public async Task<IServiceResult> GetActiveStationsAsync()
        {
            var stations = await unitOfWork.StationRepository.GetActiveStationsAsync();
            
            // Map to DTO to prevent circular reference
            var stationDTOs = stations.Select(s => s.ToViewStationDTO()).ToList();
            
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_READ_CODE,
                Message = Const.SUCCESS_READ_MSG,
                Data = stationDTOs
            };
        }

        public async Task<IServiceResult> GetInactiveStationsAsync()
        {
            var stations = await unitOfWork.StationRepository.GetInactiveStationsAsync();
            
            // Map to DTO to prevent circular reference
            var stationDTOs = stations.Select(s => s.ToViewStationDTO()).ToList();
            
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_READ_CODE,
                Message = Const.SUCCESS_READ_MSG,
                Data = stationDTOs
            };
        }

        public async Task<IServiceResult> UpdateIsActiveAsync(Guid stationId, bool isActive)
        {
            var success = await unitOfWork.StationRepository.UpdateIsActiveAsync(stationId, isActive);
            if (!success)
                return new ServiceResult { StatusCode = Const.WARNING_NO_DATA_CODE, Message = "Không tìm thấy trạm" };
            return new ServiceResult { StatusCode = Const.SUCCESS_UPDATE_CODE, Message = "Cập nhật trạng thái trạm thành công" };
        }

        public async Task<IServiceResult> GetStationsByVehicleModelAsync(Guid vehicleModelId)
        {
            try
            {
                // AC1: Kiểm tra model có tồn tại không
                var model = await unitOfWork.VehicleModelRepository.GetVehicleModelByIdAsync(vehicleModelId);
                if (model == null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.ERROR_VALIDATION_CODE,
                        Message = "Model does not exist in our system"
                    };
                }

                // Lấy danh sách trạm và số lượng xe available
                var stationsWithCount = await unitOfWork.StationRepository.GetStationsByVehicleModelAsync(vehicleModelId);

                // AC3: Nếu không có trạm nào
                if (stationsWithCount == null || !stationsWithCount.Any())
                {
                    return new ServiceResult
                    {
                        StatusCode = 204,
                        Message = "No stations found for this model"
                    };
                }

                // AC4: Map sang DTO với đầy đủ thông tin
                var response = stationsWithCount.Select(x => new StationWithAvailableVehiclesResponse
                {
                    StationId = x.Station.StationId,
                    Name = x.Station.Name,
                    Address = x.Station.Address,
                    Capacity = x.Station.Capacity,
                    Lat = x.Station.Lat,
                    Long = x.Station.Long,
                    AvailableVehicleCount = x.AvailableVehicleCount
                }).ToList();

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = "Lấy danh sách trạm thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"Lỗi khi lấy danh sách trạm: {innerMessage}"
                };
            }
        }
    }
}
