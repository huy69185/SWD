using AuthenticationApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructure.Data
{
    public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : DbContext(options)
    {
        public DbSet<AppUser> Users { get; set; }
        public DbSet<BugReport> BugReports { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.ToTable("UserAccount");
                entity.HasKey(e => e.UserAccountID);
                entity.Property(e => e.UserAccountID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PhoneVerified).HasDefaultValue(false);
            });
            modelBuilder.Entity<BugReport>(entity =>
            {
                entity.Property(e => e.BugReportId).HasDefaultValueSql("NEWID()");
            });
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.NotificationId).HasDefaultValueSql("NEWID()");
            });
        }
    }
}