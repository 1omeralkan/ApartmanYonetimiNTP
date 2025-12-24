using ApartmentManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.DataAccess
{
    public class ApartmentManagementContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Flat> Flats { get; set; }
        public DbSet<FlatResident> FlatResidents { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Dues> Dues { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Update this with your actual PostgreSQL credentials
            optionsBuilder.UseNpgsql("Host=localhost;Database=ApartmentManagementDb;Username=postgres;Password=1Sjklmn90.");
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fluent API configurations if needed
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        }
    }
}
