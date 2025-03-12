namespace GrowthTracking.ShareLibrary.Exceptions
{
    public class NotFoundException(string message) : Exception(message)
    {
    }
    public class UnauthorizedException(string message) : Exception(message)
    {
    }

    public class ForbiddenException(string message) : Exception(message)
    {
    }
}
