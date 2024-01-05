using Microsoft.EntityFrameworkCore;

namespace EMSWebApi.Data
{
    public class EMSWebApiContext : DbContext
    {
        public EMSWebApiContext(DbContextOptions<EMSWebApiContext> options)
            : base(options)
        {
        }

        public DbSet<EMS.Data.Models.User> User { get; set; } = default!;
    }
}
