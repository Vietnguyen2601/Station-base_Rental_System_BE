using EVStationRental.Repositories.IRepositories;
using System;
using System.Threading.Tasks;

namespace EVStationRental.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        IAccountRepository AccountRepository { get; }
        IVehicleRepository VehicleRepository { get; }
        IVehicleModelRepository VehicleModelRepository { get; }
        IStationRepository StationRepository { get; }
    }
}