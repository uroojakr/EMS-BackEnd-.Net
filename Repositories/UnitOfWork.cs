
using EMS.Data.Interfaces;
using EMS.Data.Models;
using Microsoft.Extensions.Logging;


namespace EMS.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EMSDbContext _context;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        public UnitOfWork(
            EMSDbContext context,
            ILogger<UnitOfWork> logger,
            ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = logger;
            _loggerFactory = loggerFactory;

            // Inject logger instances when creating repositories
            UserRepository = new Repository<User>(_context, _loggerFactory.CreateLogger<Repository<User>>());
            EventsRepository = new Repository<Events>(_context, _loggerFactory.CreateLogger<Repository<Events>>());
            ReviewRepository = new Repository<Review>(_context, _loggerFactory.CreateLogger<Repository<Review>>());
            TicketRepository = new Repository<Ticket>(_context, _loggerFactory.CreateLogger<Repository<Ticket>>());
            VendorRepository = new Repository<Vendor>(_context, _loggerFactory.CreateLogger<Repository<Vendor>>());
        }


        public IRepository<User> UserRepository { get; }
        public IRepository<Events> EventsRepository { get; }
        public IRepository<Review> ReviewRepository { get; }
        public IRepository<Ticket> TicketRepository { get; }
        public IRepository<Vendor> VendorRepository { get; }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                throw;
            }
        }

        public void Dispose()
        {

            _context.Dispose();
        }

        public IRepository<TEntityModel> GetRepository<TEntityModel>() where TEntityModel : class
        {
            return new Repository<TEntityModel>(_context, _loggerFactory.CreateLogger<Repository<TEntityModel>>());
        }


    }
}
