using ParentManageApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ParentManageApi.Infrastructure.Data
{
    public class ParentManageDbContext(DbContextOptions<ParentManageDbContext> options) : DbContext(options)
    {
        public DbSet<Parent> Parents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Parent>(entity =>
            {
                entity.HasKey(e => e.ParentId);
                entity.Property(e => e.ParentId).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasQueryFilter(p => !p.IsDeleted);
            });
        }
    }
}