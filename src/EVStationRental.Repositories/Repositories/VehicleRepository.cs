using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EVStationRental.Repositories.DBContext;
using EVStationRental.Repositories.IRepositories;
using EVStationRental.Repositories.Models;
using EVStationRental.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EVStationRental.Repositories.Repositories
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        private readonly ElectricVehicleDContext _context;

        public VehicleRepository(ElectricVehicleDContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            return await _context.Set<Vehicle>().ToListAsync();
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(Guid vehicleId)
        {
            return await _context.Set<Vehicle>().FindAsync(vehicleId);
        }

        public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
        {
            _context.Set<Vehicle>().Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<Vehicle?> UpdateVehicleAsync(Vehicle vehicle)
        {
            _context.Set<Vehicle>().Update(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }
    }
}
