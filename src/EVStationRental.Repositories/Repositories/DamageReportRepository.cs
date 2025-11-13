using EVStationRental.Repositories.DBContext;
using EVStationRental.Repositories.IRepositories;
using EVStationRental.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Repositories.Repositories
{
    public class DamageReportRepository : IDamageReportRepository
    {
        private readonly ElectricVehicleDBContext _context;

        public DamageReportRepository(ElectricVehicleDBContext context)
        {
            _context = context;
        }

        public async Task<DamageReport> CreateDamageReportAsync(DamageReport damageReport)
        {
            _context.Set<DamageReport>().Add(damageReport);
            await _context.SaveChangesAsync();
            return damageReport;
        }

        public async Task<List<DamageReport>> GetAllDamageReportsAsync()
        {
            return await _context.Set<DamageReport>().ToListAsync();
        }

        public async Task<DamageReport?> GetDamageReportByDamageIdAsync(Guid damageId)
        {
            return await _context.Set<DamageReport>().FirstOrDefaultAsync(v => v.DamageId == damageId);
        }

        public async Task<DamageReport?> GetDamageReportByOrderIdAsync(Guid orderId)
        {
            return await _context.Set<DamageReport>().FirstOrDefaultAsync(v => v.OrderId == orderId);
        }

        public async Task<List<DamageReport>> GetDamageReportsByVehicleIdAsync(Guid vehicleId)
        {
            return await _context.Set<DamageReport>().Where(d => d.VehicleId == vehicleId).ToListAsync();
        }

        public async Task<bool> HardDeleteDamageReportAsync(Guid damageId)
        {
            var damageReport = await _context.Set<DamageReport>().FindAsync(damageId);
            if (damageReport == null) return false;
            _context.Set<DamageReport>().Remove(damageReport);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteDamageReportAsync(Guid damageId)
        {
            var damageReport = await _context.Set<DamageReport>().FindAsync(damageId);
            if (damageReport == null) return false;
            damageReport.Isactive = false;
            _context.Set<DamageReport>().Update(damageReport);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DamageReport?> UpdateDamageReportAsync(DamageReport damageReport)
        {
            _context.Set<DamageReport>().Update(damageReport);
            await _context.SaveChangesAsync();
            return damageReport;
        }
    }
}
