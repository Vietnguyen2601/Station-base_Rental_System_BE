using EVStationRental.Common.DTOs;
using EVStationRental.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Services.InternalServices.IServices.IAccountServices
{
    public interface IAccountService
    {
        Task<IServiceResult> GetAccountByIdAsync(Guid id);
        Task<IServiceResult> GetAllAccountsAsync();
        Task<IServiceResult> SetAccountRolesAsync(Guid accountId, List<Guid> roleIds);
        Task<IServiceResult> SetAdminRoleAsync(Guid accountId);
    }
}
