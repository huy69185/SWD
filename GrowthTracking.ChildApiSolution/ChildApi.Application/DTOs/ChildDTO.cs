using System;

namespace ChildApi.Application.DTOs
{
    public record ChildDTO(
        Guid? Id,
        Guid ParentId,
        string FullName,
        DateTime DateOfBirth,
        string Gender,
        decimal BirthWeight,
        decimal BirthHeight,
        string? AvatarUrl
    );
}