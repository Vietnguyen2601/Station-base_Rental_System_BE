using System;
using System.Threading.Tasks;
using EVStationRental.Repositories.DBContext;
using EVStationRental.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace EVStationRental.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ElectricVehicleDContext _context;

        private IAccountRepository accountRepository;
        private IVehicleRepository vehicleRepository;
        private IVehicleModelRepository vehicleModelRepository;
        private IStationRepository stationRepository;

        public UnitOfWork()
        {
          _context = new ElectricVehicleDContext();
        }

        public IAccountRepository AccountRepository
        {
            get
            {
                return accountRepository ??= new Repositories.AccountRepository(_context);
            }
        }

        public IVehicleRepository VehicleRepository
        {
            get
            {
                return vehicleRepository ??= new Repositories.VehicleRepository(_context);
            }
        }

        public IVehicleModelRepository VehicleModelRepository
        {
            get
            {
                return vehicleModelRepository ??= new Repositories.VehicleModelRepository(_context);
            }
        }

        public IStationRepository StationRepository
        {
            get
            {
                return stationRepository ??= new Repositories.StationRepository(_context);
            }
        }
    }
}