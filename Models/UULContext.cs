using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace uul_api.Models {
    public class UULContext : DbContext {
        public UULContext(DbContextOptions<UULContext> options)
            : base(options) {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Habitant> Habitants { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Tower> Towers { get; set; }
        public DbSet<SpecialFloor> SpecialFloors { get; set; }
        public DbSet<BannedApartment> BannedApartments { get; set; }
        public DbSet<Rules> Rules { get; set; }
        public DbSet<Gym> Gyms { get; set; }
        public DbSet<News> News { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Habitants)
                .WithOne(h => h.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rules>()
                .HasMany(r => r.Towers)
                .WithOne(t => t.Rules)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Rules>()
                .HasMany(r => r.SpecialFloors)
                .WithOne(t => t.Rules)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Rules>()
                .HasMany(r => r.BannedApartments)
                .WithOne(t => t.Rules)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Rules>()
                .HasMany(r => r.Gyms)
                .WithOne(t => t.Rules)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

