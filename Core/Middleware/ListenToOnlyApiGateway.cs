using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {

        public async Task InvokeAsync(HttpContext context)
        {
            //Extract specific header from request
            var signedHeader = context.Request.Headers["Api-Gateway"];

            //null means, the request is not from API Gateway
            if(signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Service Unavailable. Requests are only accepted from the API Gateway.");
                return;
            }
            else
            {
                //proceed to next middleware
                await next(context);
            }

        }


    }
}
