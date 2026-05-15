using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TechExercise.Application.Exceptions;
using TechExercise.WebApi.Middleware;

namespace TechExercise.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private static async Task<string> ReadBodyAsync(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn401_WhenUnauthorizedAccessException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new UnauthorizedAccessException("Access denied"),
            loggerMock.Object
        );

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        context.Response.ContentType.Should().Be("application/json");

        var body = await ReadBodyAsync(context);
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        json.GetProperty("error").GetString().Should().Be("Access denied");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn400_WhenValidationException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new ValidationException("Title is required"),
            loggerMock.Object
        );

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        context.Response.ContentType.Should().Be("application/json");

        var body = await ReadBodyAsync(context);
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        json.GetProperty("error").GetString().Should().Be("Title is required");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn409_WhenConflictException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new ConflictException("Resource already exists"),
            loggerMock.Object
        );

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
        context.Response.ContentType.Should().Be("application/json");

        var body = await ReadBodyAsync(context);
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        json.GetProperty("error").GetString().Should().Be("Resource already exists");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn500_WhenGenericException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new Exception("Something went wrong"),
            loggerMock.Object
        );

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        context.Response.ContentType.Should().Be("application/json");

        var body = await ReadBodyAsync(context);
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        json.GetProperty("error").GetString().Should().Be("An internal server error occurred");
    }

    [Fact]
    public async Task InvokeAsync_ShouldPassThrough_WhenNoException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var invoked = false;

        var middleware = new ExceptionHandlingMiddleware(
            ctx =>
            {
                invoked = true;
                ctx.Response.StatusCode = 200;
                return Task.CompletedTask;
            },
            loggerMock.Object
        );

        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        invoked.Should().BeTrue();
        context.Response.StatusCode.Should().Be(200);
    }
}
