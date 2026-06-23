using System.Net;
using TaskManager.Core.Exceptions;

namespace TaskManager.API.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        private readonly ILogger<ExceptionMiddleware> _logger = logger;
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var errorId = Guid.NewGuid();
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";
                var error = new
                {
                    errorId,
                    ex.Message
                };
                await context.Response.WriteAsJsonAsync(error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception. ErrorId: {ErrorId}", errorId);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var error = new
                {
                    errorId,
                    Message = $"Something went wrong!"
                };
                await context.Response.WriteAsJsonAsync(error);
            }

        }
    }
}