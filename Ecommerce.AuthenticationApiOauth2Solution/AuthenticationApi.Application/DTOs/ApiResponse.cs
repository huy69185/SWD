namespace AuthenticationApi.Application.DTOs
{
    public class ApiResponse
    {
        public bool Flag { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }

        public ApiResponse(bool flag, string message, object? data = null)
        {
            Flag = flag;
            Message = message;
            Data = data;
        }
    }
}
