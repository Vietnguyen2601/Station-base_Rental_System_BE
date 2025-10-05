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

    }
}