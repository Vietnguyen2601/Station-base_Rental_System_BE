using System;
using System.Threading.Tasks;
using EVStationRental.Repositories.Models;

namespace EVStationRental.Repositories.IRepositories
{
    public interface IVehicleModelRepository
    {
        Task<VehicleModel?> GetVehicleModelByIdAsync(Guid id);
    }
}
