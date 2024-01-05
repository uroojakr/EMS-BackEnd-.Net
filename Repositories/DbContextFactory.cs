using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EMS.Data
{
    public class DbContextFactory : IDesignTimeDbContextFactory<EMSDbContext>
    {
        public EMSDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("EMS-DataBase") ?? throw new InvalidOperationException("Connection string 'EMS-DataBase' not found in appsettings.json");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string not found in appsettings.json");
            }

            var optionsBuilder = new DbContextOptionsBuilder<EMSDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new EMSDbContext(optionsBuilder.Options);
        }
    }
}
