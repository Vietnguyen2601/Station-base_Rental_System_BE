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
        private readonly ElectricVehicleDContext _context;

        public StationRepository(ElectricVehicleDContext context)
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
            return await _context.Set<Station>().ToListAsync();
        }

        public async Task<List<Vehicle>> GetVehiclesByStationIdAsync(Guid stationId)
        {
            return await _context.Vehicles
                .Where(v => v.StationId == stationId)
                .ToListAsync();
        }

        public async Task<bool> AddVehiclesToStationAsync(Guid stationId, List<Guid> vehicleIds)
        {
            var vehicles = await _context.Vehicles.Where(v => vehicleIds.Contains(v.VehicleId)).ToListAsync();
            if (vehicles.Count == 0) return false;
            foreach (var vehicle in vehicles)
            {
                vehicle.StationId = stationId;
                _context.Entry(vehicle).Property(v => v.StationId).IsModified = true;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Station?> UpdateStationAsync(Station station)
        {
            _context.Stations.Update(station);
            await _context.SaveChangesAsync();
            return station;
        }
    }
}
