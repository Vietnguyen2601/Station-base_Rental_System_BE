using System;
using System.Threading.Tasks;
using EVStationRental.Repositories.DBContext;
using EVStationRental.Repositories.IRepositories;
using EVStationRental.Repositories.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}
