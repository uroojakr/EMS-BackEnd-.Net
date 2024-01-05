

using EMS.Data.Interfaces;
using EMS.Data.Models;
using Microsoft.Extensions.Logging;

namespace EMS.Data
{
    public class VendorRepository : Repository<Vendor>, IVendorRepository
    {
        public VendorRepository(EMSDbContext context, ILogger<Repository<Vendor>> logger) : base(context, logger)
        {
        }
    }
}
