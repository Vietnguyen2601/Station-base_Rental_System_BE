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
                .ToListAsync();
        }

        public async Task<Account?> GetAccountByAccountRole()
        {
            return await _context.Accounts
                .Include(a => a.AccountRoles)
                .FirstOrDefaultAsync();
        }

        public async Task<Account?> GetAccountWithDetailsAsync(Guid accountId)
        {
            return await _context.Accounts
                .Include(a => a.AccountRoles)
                .Include(a => a.Feedbacks)
                .Include(a => a.Licenses)
                .Include(a => a.OrderCustomers)
                .Include(a => a.OrderStaffs)
                .Include(a => a.Reports)
                .Include(a => a.StaffRevenues)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        public async Task<Account?> GetAccountByIdAsync(Guid accountId)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        public async Task<Account?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _context.Accounts
                .Where(a => a.Username == usernameOrEmail || a.Email == usernameOrEmail)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _context.Accounts
                .Include(a => a.AccountRoles)
                .ThenInclude(ar => ar.Role)
                .ToListAsync();
        }
    }
}
