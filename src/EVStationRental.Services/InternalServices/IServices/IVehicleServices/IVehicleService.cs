using System.Threading.Tasks;
using EVStationRental.Common.DTOs.VehicleDTOs;
using EVStationRental.Services.Base;
using System;

namespace EVStationRental.Services.InternalServices.IServices.IVehicleServices
{
    public interface IVehicleService
    {
        Task<IServiceResult> GetAllVehiclesAsync();
        Task<IServiceResult> GetVehicleByIdAsync(Guid id);
        Task<IServiceResult> CreateVehicleAsync(CreateVehicleRequestDTO dto);
        Task<IServiceResult> UpdateVehicleAsync(Guid vehicleId, UpdateVehicleRequestDTO dto);
    }
}
