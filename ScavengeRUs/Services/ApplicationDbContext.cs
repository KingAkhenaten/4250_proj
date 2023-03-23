using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ScavengeRUs.Models.Entities;

namespace ScavengeRUs.Data
{
    /// <summary>
    /// This is the interface to connects to the database
    /// 
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Location> Location => Set<Location>();
        public DbSet<Hunt> Hunts => Set<Hunt>();
        public DbSet<HuntLocation> HuntLocation => Set<HuntLocation>();
        public DbSet<AccessCode> AccessCodes => Set<AccessCode>();


        /// <summary>
        /// This sets up with cascade contraint when deleting related data in the DB
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Hunt>()
                .HasMany<AccessCode>(a => a.AccessCodes)
                .WithOne(b => b.Hunt)
                .HasForeignKey(b => b.HuntId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Hunt>()
                .HasMany<ApplicationUser>(a => a.Players)
                .WithOne(b => b.Hunt)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<ApplicationUser>()
                .HasOne(a => a.AccessCode)
                .WithMany(a => a.Users)     
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<ApplicationUser>()
                .HasOne(a => a.Hunt)
                .WithMany(a => a.Players)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<AccessCode>()
                .HasMany<ApplicationUser>(a => a.Users)
                .WithOne(a => a.AccessCode)

                .OnDelete(DeleteBehavior.SetNull);            
            builder.Entity<AccessCode>()
                .HasOne(a => a.Hunt)
                .WithMany(a => a.AccessCodes)
                .HasForeignKey(a => a.HuntId)
                .OnDelete(DeleteBehavior.SetNull);

            


            
        }
    }
}