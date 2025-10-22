using EVStationRental.Repositories.Base;
using EVStationRental.Repositories.Models;

namespace EVStationRental.Repositories.IRepositories
{
    public interface IReportRepository : IGenericRepository<Report>
    {
        Task<List<Report>> GetByAccountIdAsync(Guid accountId);
    }
}
