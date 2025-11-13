using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EVStationRental.Repositories.DBContext;
using EVStationRental.Repositories.IRepositories;
using EVStationRental.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EVStationRental.Repositories.Repositories
{
    public class StationRepository : IStationRepository
    {
        private readonly ElectricVehicleDBContext _context;

        public StationRepository(ElectricVehicleDBContext context)
        {
            _context = context;
        }

        public async Task<Station?> GetStationByIdAsync(Guid id)
        {
            return await _context.Set<Station>().FindAsync(id);
        }

        public async Task<Station> CreateStationAsync(Station station)
        {
            _context.Set<Station>().Add(station);
            await _context.SaveChangesAsync();
            return station;
        }

        public async Task<List<Station>> GetAllStationsAsync()
        {
            return await _context.Stations
                .Include(s => s.Vehicles)
                .Where(s => s.Isactive)
                .ToListAsync();
        }

        public async Task<List<Vehicle>> GetVehiclesByStationIdAsync(Guid stationId)
        {
            return await _context.Vehicles
                .Where(v => v.StationId == stationId)
                .ToListAsync();
        }

        public async Task<Station?> UpdateStationAsync(Station station)
        {
            _context.Stations.Update(station);
            await _context.SaveChangesAsync();
            return station;
        }

        public async Task<bool> SoftDeleteStationAsync(Guid stationId)
        {
            var station = await GetStationByIdAsync(stationId);
            if (station == null) return false;
            station.Isactive = false;
            _context.Set<Station>().Update(station);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Station>> GetActiveStationsAsync()
        {
            return await _context.Set<Station>().Where(s => s.Isactive).ToListAsync();
        }

        public async Task<List<Station>> GetInactiveStationsAsync()
        {
            return await _context.Set<Station>().Where(s => !s.Isactive).ToListAsync();
        }

        public async Task<bool> UpdateIsActiveAsync(Guid stationId, bool isActive)
        {
            var station = await GetStationByIdAsync(stationId);
            if (station == null) return false;
            station.Isactive = isActive;
            _context.Set<Station>().Update(station);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddVehiclesToStationAsync(Guid stationId, List<Guid> vehicleIds)
        {
            var vehicles = await _context.Vehicles.Where(v => vehicleIds.Contains(v.VehicleId)).ToListAsync();
            if (vehicles.Count == 0) return false;
            foreach (var vehicle in vehicles)
            {
                vehicle.StationId = stationId;
                _context.Vehicles.Update(vehicle);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<(Station Station, int AvailableVehicleCount)>> GetStationsByVehicleModelAsync(Guid vehicleModelId)
        {
            var vehiclesByModel = await _context.Vehicles
                .Include(v => v.Station)
                .Where(v => v.ModelId == vehicleModelId 
                         && v.Isactive == true 
                         && (v.Status == Common.Enums.EnumModel.VehicleStatus.AVAILABLE 
                             || v.Status == Common.Enums.EnumModel.VehicleStatus.CHARGING)
                         && v.StationId != null)
                .ToListAsync();

            var stationsWithCount = vehiclesByModel
                .GroupBy(v => v.Station)
                .Select(g => (
                    Station: g.Key!,
                    AvailableVehicleCount: g.Count(v => v.Status == Common.Enums.EnumModel.VehicleStatus.AVAILABLE)
                ))
                .OrderBy(x => x.Station.Name)
                .ToList();

            return stationsWithCount;
        }
    }
}
