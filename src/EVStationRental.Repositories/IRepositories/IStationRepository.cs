using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EVStationRental.Repositories.Models;

namespace EVStationRental.Repositories.IRepositories
{
    public interface IStationRepository
    {
        Task<Station?> GetStationByIdAsync(Guid id);
        Task<Station> CreateStationAsync(Station station);
        Task<List<Station>> GetAllStationsAsync();
        Task<List<Vehicle>> GetVehiclesByStationIdAsync(Guid stationId);
        Task<bool> AddVehiclesToStationAsync(Guid stationId, List<Guid> vehicleIds);
    }
}
