
using EMS.Data.Interfaces;
using EMS.Data.Models;
using Microsoft.Extensions.Logging;

namespace EMS.Data
{
    public class VendorEventRepository : Repository<VendorEvent>, IVendorEventRepository
    {
        public VendorEventRepository(EMSDbContext context, ILogger<Repository<VendorEvent>> logger) : base(context, logger)
        {
        }
    }
}
