using Core.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Core.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //default variables
            string message = "Sorry, internal server error occurred. Kindly try again";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                //Check if response status code is 429
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Too Many Requests";
                    message = "You have sent too many requests in a given amount of time. Please try again later.";
                    statusCode = StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, title, message, statusCode);
                }

                //if response status code is 401
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Unauthorized";
                    message = "You are not authorized to access this resource. Kindly authenticate and try again.";
                    statusCode = StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context, title, message, statusCode);
                }

                //if response status code is 403
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Forbidden";
                    message = "You do not have permission to access this resource.";
                    statusCode = StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {
                // Log Original Exceptions / Console, file , debugger
                LogExcepitons.LogExceptions(ex);

                //check if exception is of type timeout
                if (ex is TimeoutException || ex is TaskCanceledException)
                {
                    title = "Request Timeout";
                    message = "The request has timed out. Please try again later.";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }

                //if exception is caught.
                //if none of the exceptions then do default
                await ModifyHeader(context, title, message, statusCode);
            }
        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            // display scary-free message to client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
            {
                Detail = message,
                Status = statusCode,
                Title = title
            }), CancellationToken.None);
            return;

        }
    }
}
