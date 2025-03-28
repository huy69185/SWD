using CloudinaryDotNet.Actions;
using GrowthTracking.DoctorSolution.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GrowthTracking.DoctorSolution.Infrastructure.Cloudinary
{
    using CloudinaryDotNet;

    public class CloudinaryService : IFileStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> cloudinarySettings)
        {
            var settings = cloudinarySettings.Value;
            var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<FileUploadResult> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = "File is empty or null."
                };
            }

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream)
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = uploadResult.Error.Message
                };
            }

            return new FileUploadResult
            {
                Success = true,
                Url = uploadResult.SecureUrl?.ToString(),
                PublicId = uploadResult.PublicId
            };
        }
    }
}
