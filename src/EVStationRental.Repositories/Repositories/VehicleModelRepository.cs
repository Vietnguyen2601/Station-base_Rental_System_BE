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
        private readonly ElectricVehicleDBContext _context;

        public VehicleModelRepository(ElectricVehicleDBContext context)
        {
            _context = context;
        }

        public async Task<VehicleModel> CreateVehicleModelAsync(VehicleModel vehicleModel)
        {
            _context.Set<VehicleModel>().Add(vehicleModel);
            await _context.SaveChangesAsync();
            return vehicleModel;
        }

        public async Task<List<VehicleModel?>> GetActiveVehicleModelsAsync()
        {
            return await _context.Set<VehicleModel>().Where(vt => vt.Isactive).ToListAsync();
        }

        public async Task<List<VehicleModel>> GetAllVehicleModelsAsync()
        {
            return await _context.VehicleModels
                .Include(vm => vm.Type)
                .Where(vm => vm.Isactive)
                .ToListAsync();
        }

        public async Task<VehicleModel?> GetVehicleModelByIdAsync(Guid id)
        {
            return await _context.Set<VehicleModel>().FindAsync(id);
        }

        public async Task<bool> HardDeleteVehicleModelAsync(Guid id)
        {
            var vehicleModel = _context.Set<VehicleModel>().Find(id);
            if (vehicleModel == null) return false;
            _context.Set<VehicleModel>().Remove(vehicleModel);
            await _context.SaveChangesAsync();
            return true;


        }

        public async Task<bool> SoftDeleteVehicleModelAsync(Guid id)
        {
            var vehicleModel = await GetVehicleModelByIdAsync(id);
            if (vehicleModel == null) return false;
            vehicleModel.Isactive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<VehicleModel?> UpdateVehicleModelAsync(VehicleModel vehicleModel)
        {
            _context.Set<VehicleModel>().Update(vehicleModel);
            await _context.SaveChangesAsync();
            return vehicleModel;
        }
    }
}
