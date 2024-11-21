using eCommerce.ShareLibrary.Logs;
using eCommerce.ShareLibrary.Response;
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

            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                //Log original exceptions/File, Debugger, Console
                LogHandler.LogExceptions(ex);

                //Check if exception time out ==> status 408
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    var message = "Request time out!!!Please try again";
                    var statusCode = StatusCodes.Status408RequestTimeout;
                    await HandleExceptionAsync(context, message, statusCode);
                }
                else
                {
                    await HandleExceptionAsync(context, ex);
                }  
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

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await context.Response.WriteAsync(new ApiResponse
                {
                    Success = false,
                    Message = exception.Message
                }.ToString() ?? string.Empty);
            }
        }
    }
}