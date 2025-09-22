//using System;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;

//namespace EVStationRental.Repositories.UnitOfWork
//{
//    public class UnitOfWork : IUnitOfWork
//    {
//        private readonly DbContext _context;
//        private bool _disposed = false;

//        // Add your repository fields here, for example:
//        // private IStationRepository _stations;
//        // private IBookingRepository _bookings;
//        // private IUserRepository _users;

//        public UnitOfWork(DbContext context)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//        }

//        // Implement your repository properties here, for example:
//        /*
//        public IStationRepository Stations
//        {
//            get
//            {
//                if (_stations == null)
//                {
//                    _stations = new StationRepository(_context);
//                }
//                return _stations;
//            }
//        }
//        */

//        public int Save()
//        {
//            return _context.SaveChanges();
//        }

//        public async Task<int> SaveAsync()
//        {
//            return await _context.SaveChangesAsync();
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!_disposed)
//            {
//                if (disposing)
//                {
//                    _context.Dispose();
//                }
//            }
//            _disposed = true;
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }
//    }
//}