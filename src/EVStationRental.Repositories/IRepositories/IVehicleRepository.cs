using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EVStationRental.Repositories.Models;
using EVStationRental.Repositories.Base;

namespace EVStationRental.Repositories.IRepositories
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle?> GetVehicleByIdAsync(Guid vehicleId);
        Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);
        Task<Vehicle?> UpdateVehicleAsync(Vehicle vehicle);
    }
}
