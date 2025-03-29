using GrowthTracking.ShareLibrary.Validation;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
    public record NotificationDTO(
        [GuidValidation] Guid NotificationId,
        [Required] Guid UserId,
        [Required] string NotificationType,
        [Required] string Content,
        [Required] string Status,
        DateTime? SentAt = null,
        DateTime? CreatedAt = null,
        DateTime? UpdatedAt = null
    );
}