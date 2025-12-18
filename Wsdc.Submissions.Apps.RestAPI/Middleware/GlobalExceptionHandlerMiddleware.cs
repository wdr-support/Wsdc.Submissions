using System.Net;
using System.Text.Json;
using Wsdc.Submissions.Core;

namespace Wsdc.Submissions.Apps.RestAPI.Middleware;

/// <summary>
/// Global exception handler middleware for catching and formatting unhandled exceptions
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var errorId = Guid.NewGuid().ToString();
            _logger.LogError(ex, "Unhandled exception occurred. ErrorId: {ErrorId}, Message: {Message}", errorId, ex.Message);
            await HandleExceptionAsync(context, errorId);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, string errorId)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = ServiceResponse<object>.Failure(
            "INTERNAL_SERVER_ERROR",
            $"An unexpected error occurred while processing your request. Please contact support with this ticket number: {errorId}",
            null,
            ErrorSeverity.Critical
        );

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}

