using System;
using System.Threading.Tasks;
using EVStationRental.Repositories.DBContext;
using EVStationRental.Repositories.IRepositories;
using EVStationRental.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace EVStationRental.Repositories.Repositories
{
    public class VehicleModelRepository : IVehicleModelRepository
    {
        private readonly ElectricVehicleDContext _context;

        public VehicleModelRepository(ElectricVehicleDContext context)
        {
            _context = context;
        }

        public async Task<VehicleModel?> GetVehicleModelByIdAsync(Guid id)
        {
            return await _context.Set<VehicleModel>().FindAsync(id);
        }
    }
}
