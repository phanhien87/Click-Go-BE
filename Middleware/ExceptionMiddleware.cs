using Click_Go.Helper;
using System.Net;
using System.Text.Json;

namespace Click_Go.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception caught in middleware.");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            string message = "Internal Server Error. Please contact support.";

            switch (exception)
            {
                case AppException ex:
                    statusCode = HttpStatusCode.Conflict; // 409
                    message = ex.Message;
                    break;

                case NotFoundException ex:
                    statusCode = HttpStatusCode.NotFound; // 404
                    message = ex.Message;
                    break;

                case UnauthorizedAccessException ex:
                    statusCode = HttpStatusCode.Unauthorized; // 401
                    message = ex.Message;
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            };

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
