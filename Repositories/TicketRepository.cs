
using EMS.Data.Interfaces;
using EMS.Data.Models;
using Microsoft.Extensions.Logging;

namespace EMS.Data
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        public TicketRepository(EMSDbContext context, ILogger<Repository<Ticket>> logger) : base(context, logger)
        {
        }
    }
}
