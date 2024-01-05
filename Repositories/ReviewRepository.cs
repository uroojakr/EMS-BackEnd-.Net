using EMS.Data.Interfaces;
using EMS.Data.Models;
using Microsoft.Extensions.Logging;

namespace EMS.Data
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(EMSDbContext context, ILogger<Repository<Review>> logger) : base(context, logger)
        {
        }
    }
}
