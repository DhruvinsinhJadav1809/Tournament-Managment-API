using System.Net;
using System.Text.Json;
using GamesAPI.Api.Exceptions;

namespace GamesAPI.Api.Middleware
{
    public class ExceptionMiddleware

    {
        private readonly
            RequestDelegate _next;


        public ExceptionMiddleware(
     RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
    HttpContext context,
    ILogService logService)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(
                    context,
                    ex,
                    logService);
            }
        }

        private async Task
            HandleExceptionAsync(
                HttpContext context,
                Exception exception,
                ILogService logService)
        {
            var userIdClaim =
    context.User.FindFirst(
        System.Security.Claims
            .ClaimTypes
            .NameIdentifier)
    ?.Value;

            Guid? userId = null;

            if (Guid.TryParse(
                    userIdClaim,
                    out var parsedUserId))
            {
                userId = parsedUserId;
            }
            await logService.LogAsync(
                            level: "Error",
                            message: exception.Message,
                            category: "ExceptionMiddleware",
                            functionName: "HandleExceptionAsync",
                            userId: userId,
                            exception: exception.ToString(),
                            moduleType: "API",
                            page: context.Request.Path);

            context.Response.ContentType =
                "application/json";
            var statusCode =
     exception is ApiException apiException
         ? apiException.StatusCode
         : StatusCodes.Status500InternalServerError;

            context.Response.StatusCode =
                statusCode;
            var message =
                statusCode == StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred."
                    : exception.Message;
            var response = new
            {
                success = false,
                message = message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));

        }
    }
}