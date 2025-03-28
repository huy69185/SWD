using ChildApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChildApi.Infrastructure.Data
{
    // DbContext cho Child API (quản lý bảng Child và Milestone)
    public class ChildDbContext : DbContext
    {
        public ChildDbContext(DbContextOptions<ChildDbContext> options)
            : base(options)
        {
        }

        public DbSet<Child> Children { get; set; }
        public DbSet<Milestone> Milestones { get; set; }

        // Nếu cần cấu hình thêm theo Fluent API, bạn có thể override OnModelCreating ở đây.
    }
}
