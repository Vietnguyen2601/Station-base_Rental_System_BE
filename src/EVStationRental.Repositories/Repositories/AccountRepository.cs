using EVStationRental.Repositories.Base;
using EVStationRental.Repositories.DBContext;
using EVStationRental.Repositories.IRepositories;
using EVStationRental.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace EVStationRental.Repositories.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository()
        {
        }

        public AccountRepository(ElectricVehicleDContext context)
            => _context = context;

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            return await _context.Accounts
                .Where(a => a.Username == username)
                .FirstOrDefaultAsync();
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _context.Accounts
                .Where(a => a.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<Account?> GetByContactNumberAsync(string contactNumber)
        {
            return await _context.Accounts
                .Where(a => a.ContactNumber == contactNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Account>> GetAllActiveAccountsAsync()
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .Where(a => a.Isactive)
                .ToListAsync();
        }

        public async Task<Account?> GetAccountByAccountRole()
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync();
        }

        public async Task<Account?> GetAccountWithDetailsAsync(Guid accountId)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.Feedbacks)
                .Include(a => a.OrderCustomers)
                .Include(a => a.OrderStaffs)
                .Include(a => a.Reports)
                .Include(a => a.StaffRevenues)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        public async Task<Account?> GetAccountByIdAsync(Guid accountId)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        public async Task<Account?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .Where(a => a.Username == usernameOrEmail || a.Email == usernameOrEmail)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .ToListAsync();
        }

        public async Task<bool> SetAccountRolesAsync(Guid accountId, List<Guid> roleIds)
        {
            try
            {
                // Since the current database uses one-to-many relationship,
                // we can only set one role at a time
                // Taking the first role from the list
                if (roleIds == null || roleIds.Count == 0)
                    return false;

                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountId == accountId);

                if (account == null)
                    return false;

                // Set the first role (current schema only supports one role per account)
                account.RoleId = roleIds.First();
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Account?> GetAccountWithRolesAsync(Guid accountId)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        public async Task<bool> AddAccountRoleAsync(Guid accountId, Guid roleId)
        {
            try
            {
                // Since the current database uses one-to-many relationship,
                // we update the account's RoleId
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountId == accountId);

                if (account == null)
                    return false;

                account.RoleId = roleId;
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
