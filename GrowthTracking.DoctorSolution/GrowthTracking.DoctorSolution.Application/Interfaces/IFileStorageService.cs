using Microsoft.AspNetCore.Http;

namespace GrowthTracking.DoctorSolution.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<FileUploadResult> UploadFileAsync(IFormFile file);
    }

    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string? Url { get; set; }
        public string? PublicId { get; set; }
        public string? ErrorMessage { get; set; }
    }

}
