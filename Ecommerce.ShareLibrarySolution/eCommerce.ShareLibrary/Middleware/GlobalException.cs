using eCommerce.ShareLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eCommerce.ShareLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //Declare default variables
            string message = "Sorry, internal server error occurred. Kindly try again";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                switch (context.Response.StatusCode)
                {
                    case StatusCodes.Status404NotFound:
                        title = "Not Found";
                        message = "The requested resource was not found.";
                        statusCode = StatusCodes.Status404NotFound;
                        await HandleExceptionAsync(context, title, message, statusCode);
                        break;

                    case StatusCodes.Status429TooManyRequests:
                        title = "Warning";
                        message = "To many request made.";
                        statusCode = StatusCodes.Status429TooManyRequests;
                        await HandleExceptionAsync(context, title, message, statusCode);
                        break;

                    case StatusCodes.Status401Unauthorized:
                        title = "Alert";
                        message = "You are not authorized to access.";
                        statusCode = StatusCodes.Status401Unauthorized;
                        await HandleExceptionAsync(context, title, message, statusCode);
                        break;

                    case StatusCodes.Status403Forbidden:
                        title = "Out of access.";
                        message = "You are not allowed/required to access.";
                        statusCode = StatusCodes.Status403Forbidden;
                        await HandleExceptionAsync(context, title, message, statusCode);
                        break;

                    default:
                        break;
                }
             }
            catch (Exception ex)
            {
                //Log original exceptions/File, Debugger, Console
                LogHandler.LogExceptions(ex);

                //Check if exception time out ==> status 408
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Time out";
                    message = "Request time out!!!Please try again";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }
                //if none of the exceptions then do the default|| Exceptions is caugth 
                await HandleExceptionAsync(context, title, message, statusCode);   
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, string title, string message, int statusCode)
        {
            //Display message to client
            //context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title
            }), CancellationToken.None);
        }
    }
}