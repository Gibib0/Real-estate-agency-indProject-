using System.Collections.Generic;
using System.Reflection.Emit;
using BusinessLogic.Models;
using BusinessLogic.Services;
using Microsoft.EntityFrameworkCore;

namespace Integration
{
    public class RealEstateDbContext : DbContext
    {
        public RealEstateDbContext(DbContextOptions<RealEstateDbContext> options) : base(options) { }

        public DbSet<Property> Properties { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<SaveSearch> SavedSearches { get; set; }
        public DbSet<LandmarkInfo> Landmarks { get; set; }
        public DbSet<StatusChange> StatusChanges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SaveSearch>().OwnsOne(s => s.Filter);
            modelBuilder.Entity<Property>()
                .HasMany(p => p.Landmarks)
                .WithOne(l => l.Property)
                .HasForeignKey(l => l.PropertyId);

            modelBuilder.Entity<Property>()
                .HasMany(p => p.StatusHistory)
                .WithOne(s => s.Property)
                .HasForeignKey(s => s.PropertyId);
        }
    }
}
