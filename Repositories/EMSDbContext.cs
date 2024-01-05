using EMS.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.Data
{
    public class EMSDbContext : DbContext
    {

        public EMSDbContext(DbContextOptions<EMSDbContext> options) : base(options) { }

        public DbSet<Events> Events { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<VendorEvent> VendorEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // configure the many-to-many relationship between Vendor and Event through VendorEvent
            modelBuilder.Entity<VendorEvent>()
                .HasKey(ve => new { ve.VendorId, ve.EventId });

            modelBuilder.Entity<VendorEvent>()
                .HasOne(ve => ve.Vendor)
                .WithMany(v => v.VendorEvents)
                .HasForeignKey(ve => ve.VendorId);

            modelBuilder.Entity<VendorEvent>()
                .HasOne(ve => ve.Event)
                .WithMany(e => e.VendorEvents)
                .HasForeignKey(ve => ve.EventId);
            modelBuilder.Entity<Events>()
                .HasOne(ve => ve.Organizer)
                .WithMany(e => e.OrganizedEvents)
                .HasForeignKey(ve => ve.OrganizerId)
                .OnDelete(DeleteBehavior.SetNull);
            //// configure the one-to-many relationship between User and Events (Organizer)
            //modelBuilder.Entity<User>()
            //    .HasMany(u => u.OrganizedEvents)
            //    .WithOne(e => e.Organizer)
            //    .HasForeignKey(e => e.OrganizerId)
            //    .OnDelete(DeleteBehavior.SetNull); // prevent cascade delete

            // configure the one-to-many relationship between Event and Ticket
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId);

            // configure the one-to-many relationship between Event and Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Reviews)
                .HasForeignKey(r => r.EventId);



            //seeding data 
            modelBuilder.Entity<Events>().HasData(
                new Events
                {
                    Id = 1,
                    Title = "Cooking Show",
                    Description = "cooking show for beginners",
                    Date = DateTime.Now.AddDays(15),
                    Location = "ISB",
                    OrganizerId = 1,
                },
                new Events
                {
                    Id = 2,
                    Title = "Sports Event",
                    Description = "Kids Support Event",
                    Date = DateTime.Now,
                    Location = "Karachi",
                    OrganizerId = 2,
                });
            modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    Id = 1,
                    Comment = "Good event!",
                    Rating = 4,
                    UserId = 1,
                    EventId = 1,
                    VendorId = 1,
                },
                new Review
                {
                    Id = 2,
                    Comment = "Awesome event!",
                    Rating = 5,
                    UserId = 2,
                    EventId = 2,
                    VendorId = 2,
                });
            modelBuilder.Entity<Ticket>().HasData(
              new Ticket
              {
                  Id = 1,
                  UserId = 1,
                  EventId = 1,
              },
              new Ticket
              {
                  Id = 2,
                  UserId = 2,
                  EventId = 2,
              }
          );
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    UserName = "Urooj Akram",
                    Email = "urooj.akram@mail.com",
                    Password = "339fd",
                    UserType = UserType.Attendee,
                },
                new User
                {
                    Id = 2,
                    UserName = "Sana Khalid",
                    Email = "sanakhalid@mail.com",
                    Password = "4he9ju",
                    UserType = UserType.Organizer,
                });

            modelBuilder.Entity<Vendor>().HasData(
                new Vendor
                {
                    Id = 1,
                    Name = "Sports Crew",
                    Description = "Providing all the facilites for sports",
                    ContactInformation = "532-3333",
                },
                new Vendor
                {
                    Id = 2,
                    Name = "Photography",
                    Description = "Photography essentials for the events by professional cameraman",
                    ContactInformation = "339-22844",
                });

            modelBuilder.Entity<VendorEvent>().HasData(
                new VendorEvent
                {
                    Id = 1,
                    VendorId = 1,
                    EventId = 1,
                },
                 new VendorEvent
                 {
                     Id = 2,
                     VendorId = 2,
                     EventId = 2,
                 }
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
