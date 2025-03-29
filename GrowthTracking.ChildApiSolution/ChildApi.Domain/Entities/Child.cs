using GrowthTracking.ShareLibrary.Abstract;
using System;

namespace ChildApi.Domain.Entities
{
    // Entity Child: khai báo các thuộc tính theo quy ước của EF Core.
    // EF Core sẽ tự động nhận diện property tên "Id" là khoá chính.
    public class Child : BaseEntity
    {
        // Sử dụng Guid cho ChildId (override từ BaseEntity)
        public override Guid Id { get; set; }

        // Khoá ngoại liên kết đến Parent (ParentId được lấy từ Parent API qua RabbitMQ)
        public Guid ParentId { get; set; }

        public string FullName { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public decimal? BirthWeight { get; set; }

        public decimal? BirthHeight { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
