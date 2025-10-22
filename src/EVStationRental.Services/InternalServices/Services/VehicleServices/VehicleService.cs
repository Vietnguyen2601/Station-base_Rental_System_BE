using EVStationRental.Common.DTOs.VehicleDTOs;
using EVStationRental.Common.Enums.ServiceResultEnum;
using EVStationRental.Repositories.Mapper;
using EVStationRental.Repositories.Models;
using EVStationRental.Repositories.UnitOfWork;
using EVStationRental.Services.Base;
using EVStationRental.Services.InternalServices.IServices.IVehicleServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVStationRental.Services.InternalServices.Services.VehicleServices
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork unitOfWork;

        public VehicleService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IServiceResult> GetAllVehiclesAsync()
        {
            try
            {
                var vehicles = await unitOfWork.VehicleRepository.GetAllVehiclesAsync();
                if (vehicles == null || vehicles.Count == 0)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG
                    };
                }

                var vehicleDtos = vehicles.Select(v => v.ToViewVehicleDTO()).ToList();

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = vehicleDtos
                };
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"Lỗi khi lấy danh sách xe: {ex.Message} {innerMessage}"
                };
            }
        }

        public async Task<IServiceResult> GetVehicleByIdAsync(Guid id)
        {
            var vehicle = await unitOfWork.VehicleRepository.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return new ServiceResult { StatusCode = Const.WARNING_NO_DATA_CODE, Message = "Không tìm thấy xe" };
            var dto = vehicle.ToViewVehicleDTO();
            return new ServiceResult { StatusCode = Const.SUCCESS_READ_CODE, Message = Const.SUCCESS_READ_MSG, Data = dto };
        }

        public async Task<IServiceResult> CreateVehicleAsync(CreateVehicleRequestDTO dto)
        {
            // Kiểm tra model có tồn tại không
            var model = await unitOfWork.VehicleModelRepository.GetVehicleModelByIdAsync(dto.ModelId);
            if (model == null)
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_VALIDATION_CODE,
                    Message = "ModelId không hợp lệ"
                };
            }

            // Kiểm tra station có tồn tại không (nếu có truyền lên)
            if (dto.StationId != null)
            {
                var station = await unitOfWork.StationRepository.GetStationByIdAsync(dto.StationId.Value);
                if (station == null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.ERROR_VALIDATION_CODE,
                        Message = "StationId không hợp lệ"
                    };
                }
            }

            // Kiểm tra SerialNumber không bị trùng
            var existingVehicle = (await unitOfWork.VehicleRepository.GetAllVehiclesAsync())
                .FirstOrDefault(v => v.SerialNumber == dto.SerialNumber);
            if (existingVehicle != null)
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_VALIDATION_CODE,
                    Message = "SerialNumber đã tồn tại"
                };
            }
            var vehicle = dto.ToVehicle();
            var result = await unitOfWork.VehicleRepository.CreateVehicleAsync(vehicle);
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_CREATE_CODE,
                Message = Const.SUCCESS_CREATE_MSG,
                Data = result.ToViewVehicleDTO()
            };
        }

        public async Task<IServiceResult> UpdateVehicleAsync(Guid vehicleId, UpdateVehicleRequestDTO dto)
        {
            var vehicle = await unitOfWork.VehicleRepository.GetVehicleByIdAsync(vehicleId);
            if (vehicle == null)
                return new ServiceResult { StatusCode = Const.WARNING_NO_DATA_CODE, Message = "Không tìm thấy xe" };

            // Kiểm tra battery
            if (dto.BatteryLevel != null && dto.BatteryLevel <= 0)
                return new ServiceResult { StatusCode = Const.ERROR_VALIDATION_CODE, Message = "Mức pin phải là số dương" };

            // Kiểm tra StationId nếu có thay đổi
            if (dto.StationId != null)
            {
                var station = await unitOfWork.StationRepository.GetStationByIdAsync(dto.StationId.Value);
                if (station == null)
                    return new ServiceResult { StatusCode = Const.ERROR_VALIDATION_CODE, Message = "StationId không hợp lệ" };
            }

            // Kiểm tra ModelId nếu có thay đổi
            if (dto.ModelId != null)
            {
                var model = await unitOfWork.VehicleModelRepository.GetVehicleModelByIdAsync(dto.ModelId.Value);
                if (model == null)
                    return new ServiceResult { StatusCode = Const.ERROR_VALIDATION_CODE, Message = "ModelId không hợp lệ" };
                vehicle.ModelId = dto.ModelId.Value;
            }

            // Map DTO to entity chỉ với các trường có trong model Vehicle, không cập nhật VehicleId
            dto.MapToVehicle(vehicle);

            var updated = await unitOfWork.VehicleRepository.UpdateVehicleAsync(vehicle);
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_UPDATE_CODE,
                Message = Const.SUCCESS_UPDATE_MSG,
                Data = updated.ToViewVehicleDTO()
            };
        }
    }
}
