using System;

namespace ChildApi.Application.DTOs
{
    // DTO cho Child: dùng để truyền dữ liệu giữa API và service.
    public record ChildDTO(
        Guid? Id,         // Nullable cho record mới (nếu chưa có Id)
        Guid ParentId,    // Sẽ được ghi đè bằng giá trị từ RabbitMQ
        string FullName,
        DateTime DateOfBirth,
        string? Gender,
        decimal? BirthWeight,
        decimal? BirthHeight,
        string? AvatarUrl
    );
}
