using EVStationRental.Repositories.Models;

namespace EVStationRental.Repositories.IRepositories;

public interface IAuthRepository
{
    Task<Account?> GetAccountByUsernameAsync(string username);
    Task<Account?> GetAccountByIdAsync(Guid accountId);
}


