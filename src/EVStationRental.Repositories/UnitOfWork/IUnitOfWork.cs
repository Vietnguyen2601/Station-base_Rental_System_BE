using System;
using System.Threading.Tasks;

namespace EVStationRental.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Add your repository properties here, for example:
        // IStationRepository Stations { get; }
        // IBookingRepository Bookings { get; }
        // IUserRepository Users { get; }

        int Save();
        Task<int> SaveAsync();
    }
}