using System.Data.Entity;
using TechSystems.Models;

namespace TechSystems.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=TechSystemsConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; } // Изменили с Applications на ServiceRequests
        public DbSet<ServiceType> ServiceTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>().HasKey(e => e.Id);
            modelBuilder.Entity<User>().Property(e => e.Id).HasColumnName("UserID");

            // Tariff
            modelBuilder.Entity<Tariff>().ToTable("Tariffs");
            modelBuilder.Entity<Tariff>().HasKey(e => e.Id);
            modelBuilder.Entity<Tariff>().Property(e => e.Id).HasColumnName("TariffID");

            modelBuilder.Entity<Tariff>()
                .Property(t => t.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Tariff>()
                .Property(t => t.Discount)
                .HasPrecision(5, 2);

            // ServiceRequest (бывшее Application)
            modelBuilder.Entity<ServiceRequest>().ToTable("Applications");
            modelBuilder.Entity<ServiceRequest>().HasKey(e => e.Id);
            modelBuilder.Entity<ServiceRequest>().Property(e => e.Id).HasColumnName("ApplicationID");

            modelBuilder.Entity<ServiceRequest>()
                .Property(a => a.TotalCost)
                .HasPrecision(10, 2);

            // ServiceType
            modelBuilder.Entity<ServiceType>().ToTable("ServiceTypes");
            modelBuilder.Entity<ServiceType>().HasKey(e => e.Id);
            modelBuilder.Entity<ServiceType>().Property(e => e.Id).HasColumnName("ServiceTypeID");
        }
    }
}