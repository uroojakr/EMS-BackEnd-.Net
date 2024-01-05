using EMS.Data.Interfaces;
using EMS.Data.Models;
using Microsoft.Extensions.Logging;

namespace EMS.Data
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(EMSDbContext context, ILogger<Repository<User>> logger) : base(context, logger)
        {
        }
    }
}
