using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using TechExercise.Application.Exceptions;

namespace TechExercise.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            var response = new { error = ex.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning(ex, "Conflict");
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            context.Response.ContentType = "application/json";
            var response = new { error = ex.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Authorization failed");
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            var response = new { error = ex.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var response = new { error = "An internal server error occurred" };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
