using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.ShareLibrary.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //Extract specific header from the request
            var signedHeader = context.Request.Headers["Api-Gateway"];
            //The request is not coming from Api Gateway => status 503
            if(signedHeader.FirstOrDefault() is null) 
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry, service is unavailable");
                return;
            }else
            {
                await next(context);
            }
        }
    }
}
