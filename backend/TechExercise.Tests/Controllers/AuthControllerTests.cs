using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TechExercise.WebApi.Controllers;
using TechExercise.WebApi.DTOs.Auth;
using TechExercise.WebApi.Services;

namespace TechExercise.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _sut;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _sut = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Register_ShouldReturnOkWithResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "newuser",
            Email = "new@example.com",
            Password = "password123"
        };

        var expectedResponse = new LoginResponse
        {
            Token = "jwt-token",
            IdUser = 1,
            Username = "newuser",
            Email = "new@example.com"
        };

        _authServiceMock.Setup(x => x.RegisterAsync(request)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _sut.Register(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<LoginResponse>().Subject;
        response.Token.Should().Be("jwt-token");
        response.Username.Should().Be("newuser");
    }

    [Fact]
    public async Task Login_ShouldReturnOkWithResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var expectedResponse = new LoginResponse
        {
            Token = "jwt-token",
            IdUser = 1,
            Username = "testuser",
            Email = "test@example.com"
        };

        _authServiceMock.Setup(x => x.LoginAsync(request)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _sut.Login(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<LoginResponse>().Subject;
        response.Token.Should().Be("jwt-token");
    }

    [Fact]
    public async Task Register_ShouldCallServiceOnce()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "user",
            Email = "user@example.com",
            Password = "password123"
        };

        _authServiceMock.Setup(x => x.RegisterAsync(request))
            .ReturnsAsync(new LoginResponse());

        // Act
        await _sut.Register(request);

        // Assert
        _authServiceMock.Verify(x => x.RegisterAsync(request), Times.Once);
    }

    [Fact]
    public async Task Login_ShouldCallServiceOnce()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "user@example.com",
            Password = "password123"
        };

        _authServiceMock.Setup(x => x.LoginAsync(request))
            .ReturnsAsync(new LoginResponse());

        // Act
        await _sut.Login(request);

        // Assert
        _authServiceMock.Verify(x => x.LoginAsync(request), Times.Once);
    }
}
