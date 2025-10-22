using EVStationRental.Repositories.DBContext;
using EVStationRental.Repositories.IRepositories;
using EVStationRental.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace EVStationRental.Repositories.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly ElectricVehicleDContext _context;

    public AuthRepository(ElectricVehicleDContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetAccountByUsernameAsync(string username)
    {
        return await _context.Accounts
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Username == username && a.Isactive);
    }

    public async Task<Account?> GetAccountByIdAsync(Guid accountId)
    {
        return await _context.Accounts
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.AccountId == accountId && a.Isactive);
    }
}


