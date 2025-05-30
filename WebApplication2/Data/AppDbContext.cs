using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<ClientTrip> Client_Trip { get; set; }
        public DbSet<CountryTrip> Country_Trip { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("Client");
                entity.HasKey(e => e.IdClient);
                entity.Property(e => e.IdClient).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Pesel).IsUnique();
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country");
                entity.HasKey(e => e.IdCountry);
                entity.Property(e => e.IdCountry).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.ToTable("Trip");
                entity.HasKey(e => e.IdTrip);
                entity.Property(e => e.IdTrip).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ClientTrip>(entity =>
            {
                entity.ToTable("Client_Trip");
                entity.HasKey(ct => new { ct.IdClient, ct.IdTrip });

                entity.HasOne(ct => ct.Client)
                      .WithMany(c => c.ClientTrips)
                      .HasForeignKey(ct => ct.IdClient)
                      .OnDelete(DeleteBehavior.Restrict); 

                entity.HasOne(ct => ct.Trip)
                      .WithMany(t => t.ClientTrips)
                      .HasForeignKey(ct => ct.IdTrip)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CountryTrip>(entity =>
            {
                entity.ToTable("Country_Trip");
                entity.HasKey(ct => new { ct.IdCountry, ct.IdTrip });

                entity.HasOne(ct => ct.Country)
                      .WithMany(c => c.CountryTrips)
                      .HasForeignKey(ct => ct.IdCountry)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ct => ct.Trip)
                      .WithMany(t => t.CountryTrips)
                      .HasForeignKey(ct => ct.IdTrip)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}