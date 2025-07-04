using Microsoft.EntityFrameworkCore;
using CityExplorer.Models;

namespace CityExplorer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.City)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<City>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@cityexplorer.com",
                    Password = "admin123", 
                    FullName = "System Administrator",
                    Role = "Admin",
                    CreatedAt = DateTime.Now
                }
            );

            modelBuilder.Entity<City>().HasData(
                new City
                {
                    Id = 1,
                    Name = "Paris",
                    Country = "France",
                    Description = "The City of Light, famous for the Eiffel Tower, Louvre Museum, and romantic atmosphere.",
                    Price = 299.99m,
                    Duration = 5,
                    ImageUrl = "/images/paris.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new City
                {
                    Id = 2,
                    Name = "Tokyo",
                    Country = "Japan",
                    Description = "A vibrant metropolis blending traditional culture with modern technology.",
                    Price = 399.99m,
                    Duration = 7,
                    ImageUrl = "/images/tokyo.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new City
                {
                    Id = 3,
                    Name = "New York",
                    Country = "USA",
                    Description = "The Big Apple, home to Times Square, Central Park, and the Statue of Liberty.",
                    Price = 349.99m,
                    Duration = 6,
                    ImageUrl = "/images/newyork.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}
