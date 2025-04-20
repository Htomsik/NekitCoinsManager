using System.Net;
using System.Text.Json;

namespace NekitCoinsManager.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Необработанное исключение: {Message}", exception.Message);

        var code = HttpStatusCode.InternalServerError; // 500
        var result = string.Empty;

        switch (exception)
        {
            case ArgumentException _:
                code = HttpStatusCode.BadRequest; // 400
                break;
            case UnauthorizedAccessException _:
                code = HttpStatusCode.Unauthorized; // 401
                break;
            case InvalidOperationException _:
                code = HttpStatusCode.BadRequest; // 400
                break;
            case KeyNotFoundException _:
                code = HttpStatusCode.NotFound; // 404
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var response = new
        {
            StatusCode = (int)code,
            Message = exception.Message,
            // Добавляем stack trace только в разработке
            StackTrace = _environment.IsDevelopment() ? exception.StackTrace : null
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        result = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsync(result);
    }
} 