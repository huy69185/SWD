using Microsoft.AspNetCore.Http;

namespace AuthenticationApi.Application.Utils
{
    public static class Util
    {
        public static string GetCurrentDomain(HttpRequest request)
        {
            if (request == null)
            {
                throw new InvalidOperationException("Unable to determine the current domain.");
            }

            return $"{request.Scheme}://{request.Host}";
        }
    }
}
