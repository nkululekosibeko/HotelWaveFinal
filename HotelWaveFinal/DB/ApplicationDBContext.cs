using HotelWaveFinal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelWaveFinal.DB
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext>options): base(options)
        {

        }

        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public DbSet<ApplicationUser> applicationUsers { get; set; }

    }
}
