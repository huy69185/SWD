using GrowthTracking.ShareLibrary.Exceptions;
using GrowthTracking.ShareLibrary.Logs;
using GrowthTracking.ShareLibrary.Response;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace GrowthTracking.ShareLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //Declare default variables
            string message = "Sorry, internal server error occurred. Kindly try again";
            int statusCode = (int)HttpStatusCode.InternalServerError;

            try
            {
                await next(context);

                if (!context.Response.HasStarted)
                {
                    switch (context.Response.StatusCode)
                    {
                        case StatusCodes.Status429TooManyRequests:
                            message = "To many request made.";
                            statusCode = StatusCodes.Status429TooManyRequests;
                            await ModifyHeaderAsync(context, message, statusCode);
                            break;
                        case StatusCodes.Status401Unauthorized:
                            message = "You are not authorized to access.";
                            statusCode = StatusCodes.Status401Unauthorized;
                            await ModifyHeaderAsync(context, message, statusCode);
                            break;
                        case StatusCodes.Status403Forbidden:
                            message = "You are not allowed/required to access.";
                            statusCode = StatusCodes.Status403Forbidden;
                            await ModifyHeaderAsync(context, message, statusCode);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log original exceptions/File, Debugger, Console
                LogHandler.LogExceptions(ex);

                //Check if exception time out ==> status 408
                switch (ex)
                {
                    case TaskCanceledException:
                    case TimeoutException:
                        message = "Request time out!!!Please try again";
                        statusCode = StatusCodes.Status408RequestTimeout;
                        await HandleExceptionAsync(context, message, statusCode);
                        break;
                    case NotFoundException:
                        await HandleExceptionAsync(context, ex.Message, StatusCodes.Status404NotFound);
                        break;
                    case UnauthorizedException:
                        await HandleExceptionAsync(context, ex.Message, StatusCodes.Status401Unauthorized);
                        break;
                    case ForbiddenException:
                        await HandleExceptionAsync(context, ex.Message, StatusCodes.Status403Forbidden);
                        break;
                    default:
                        await HandleExceptionAsync(context, ex);
                        break;
                }
            }
        }

        private static async Task ModifyHeaderAsync(HttpContext context, string message, int statusCode)
        {
            if (!context.Response.HasStarted)
            {
                //Display message to client
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsync(new ApiResponse()
                {
                    Success = false,
                    Message = message
                }.ToString() ?? string.Empty);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await context.Response.WriteAsync(new ApiResponse
                {
                    Success = false,
                    Message = exception.Message,
                    Data = exception.StackTrace.Split("\n").ToArray()
                }.ToString() ?? string.Empty);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, string message, int statusCode)
        {
            if (!context.Response.HasStarted)
            {
                //Display message to client
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsync(new ApiResponse()
                {
                    Success = false,
                    Message = message
                }.ToString() ?? string.Empty);
            }
        }
    }
}