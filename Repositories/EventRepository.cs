
using EMS.Data.Interfaces;
using EMS.Data.Models;
using Microsoft.Extensions.Logging;

namespace EMS.Data
{
    public class EventRepository : Repository<Events>, IEventRepository
    {
        public EventRepository(EMSDbContext context, ILogger<Repository<Events>> logger) : base(context, logger)
        {
        }
    }
}
