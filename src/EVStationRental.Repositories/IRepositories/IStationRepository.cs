using System;
using System.Threading.Tasks;
using EVStationRental.Repositories.Models;

namespace EVStationRental.Repositories.IRepositories
{
    public interface IStationRepository
    {
        Task<Station?> GetStationByIdAsync(Guid id);
    }
}
