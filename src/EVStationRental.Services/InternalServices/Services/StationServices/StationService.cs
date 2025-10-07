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
            // Ki?m tra Name kh�ng b? tr�ng
            var existingStation = (await unitOfWork.StationRepository.GetAllStationsAsync())
                .FirstOrDefault(s => s.Name == dto.Name);
            if (existingStation != null)
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_VALIDATION_CODE,
                    Message = "T�n tr?m ?� t?n t?i"
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
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_READ_CODE,
                Message = Const.SUCCESS_READ_MSG,
                Data = stations
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
                    Message = "Kh�ng c� xe n�o trong tr?m n�y"
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
                    Message = "StationId kh�ng h?p l?"
                };
            }
            if (dto.VehicleIds == null || dto.VehicleIds.Count == 0)
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_VALIDATION_CODE,
                    Message = "Danh s�ch xe kh�ng h?p l?"
                };
            }
            var success = await unitOfWork.StationRepository.AddVehiclesToStationAsync(dto.StationId, dto.VehicleIds);
            if (!success)
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = "Kh�ng th? th�m xe v�o station"
                };
            }
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_CREATE_CODE,
                Message = "Th�m xe v�o station th�nh c�ng"
            };
        }
    }
}
