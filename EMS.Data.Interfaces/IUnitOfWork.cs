

using EMS.Data.Models;

namespace EMS.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> UserRepository { get; }
        IRepository<Events> EventsRepository { get; }
        IRepository<Review> ReviewRepository { get; }
        IRepository<Ticket> TicketRepository { get; }
        IRepository<Vendor> VendorRepository { get; }

        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync();
    }
}
