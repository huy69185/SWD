using System.Text.Json.Serialization;

namespace AuthenticationApi.Application.DTOs
{
    public class ApiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("data")]
        public object? Data { get; set; }

        public ApiResponse(bool success, string? message = null, object? data = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}