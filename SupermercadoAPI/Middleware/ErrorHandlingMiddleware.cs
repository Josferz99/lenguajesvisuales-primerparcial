using System.Net;
using System.Text.Json;

namespace SupermercadoAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse();

            switch (exception)
            {
                case ApplicationException ex:
                    response.Message = ex.Message;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case KeyNotFoundException ex:
                    response.Message = "Recurso no encontrado";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                case UnauthorizedAccessException ex:
                    response.Message = "No autorizado";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;

                case ArgumentException ex:
                    response.Message = ex.Message;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                default:
                    response.Message = "Error interno del servidor";
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    // En desarrollo, incluir detalles del error
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    {
                        response.Details = exception.Message;
                        response.StackTrace = exception.StackTrace;
                    }
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string? StackTrace { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    // Extension method para registrar el middleware
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}