using EVStationRental.Repositories.Base;
using EVStationRental.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Repositories.IRepositories
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account>? GetByUsernameAsync(string username);
        Task<Account>? GetByEmailAsync(string email);
        Task<Account>? GetByContactNumberAsync(string contactNumber);
        Task<List<Account>> GetAllActiveAccountsAsync();
        Task<Account>? GetAccountByAccountRole();
        Task<Account>? GetAccountWithDetailsAsync(Guid accountId);
        Task<Account>? GetAccountByIdAsync(Guid accountId);
        Task<Account>? GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<bool> SetAccountRolesAsync(Guid accountId, List<Guid> roleIds);
        Task<Account>? GetAccountWithRolesAsync(Guid accountId);
        Task<bool> AddAccountRoleAsync(Guid accountId, Guid roleId);
    }
}
