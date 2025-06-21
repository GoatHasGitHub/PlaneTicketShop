using Microsoft.EntityFrameworkCore;
using PlaneTicketShop.Core.Models;

namespace PlaneTicketShop.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Flight)
                .WithMany()
                .HasForeignKey(b => b.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "admin@admin.com",
                    UserName = "admin",
                    PasswordHash = "admin",
                    FirstName = "Admin",
                    LastName = "Admin",
                    PhoneNumber = "+1234567890",
                    Role = "Admin"
                }
            );

            modelBuilder.Entity<Flight>().HasData(
                new Flight
                {
                    Id = 1,
                    FlightNumber = "AA100",
                    DepartureCity = "New York",
                    ArrivalCity = "London",
                    DepartureTime = DateTime.UtcNow.AddDays(1),
                    ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(7),
                    Price = 500.00m,
                    AvailableSeats = 150,
                    Airline = "American Airlines",
                    AircraftType = "Boeing 777"
                },
                new Flight
                {
                    Id = 2,
                    FlightNumber = "BA200",
                    DepartureCity = "London",
                    ArrivalCity = "Paris",
                    DepartureTime = DateTime.UtcNow.AddDays(2),
                    ArrivalTime = DateTime.UtcNow.AddDays(2).AddHours(1),
                    Price = 200.00m,
                    AvailableSeats = 100,
                    Airline = "British Airways",
                    AircraftType = "Airbus A320"
                }
            );
        }
    }
} 