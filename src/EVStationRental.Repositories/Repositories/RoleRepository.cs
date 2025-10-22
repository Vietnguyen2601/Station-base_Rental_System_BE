using EVStationRental.Repositories.Base;
using EVStationRental.Repositories.DBContext;
using EVStationRental.Repositories.IRepositories;
using EVStationRental.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace EVStationRental.Repositories.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository()
        {
        }

        public RoleRepository(ElectricVehicleDContext context)
            => _context = context;

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles
                .Where(r => r.RoleName == roleName && r.Isactive)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Role>> GetAllActiveRolesAsync()
        {
            return await _context.Roles
                .Where(r => r.Isactive)
                .ToListAsync();
        }
    }
}
