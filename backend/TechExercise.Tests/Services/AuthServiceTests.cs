using System.ComponentModel.DataAnnotations;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using TechExercise.WebApi.Auth;
using TechExercise.WebApi.DTOs.Auth;
using TechExercise.WebApi.Models;
using TechExercise.WebApi.Repositories;
using TechExercise.WebApi.Services;

namespace TechExercise.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _jwtServiceMock = new Mock<IJwtService>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _sut = new AuthService(_userRepoMock.Object, _jwtServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUserAndReturnResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };

        _userRepoMock.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);
        _userRepoMock.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync(1);
        _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("jwt-token");

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("jwt-token");
        result.Username.Should().Be(request.Username);
        result.Email.Should().Be(request.Email);
        result.IdUser.Should().Be(1);

        _userRepoMock.Verify(x => x.CreateAsync(It.Is<User>(u =>
            u.Name == request.Username &&
            u.Email == request.Email &&
            !string.IsNullOrEmpty(u.PasswordHash)
        )), Times.Once);

        _jwtServiceMock.Verify(x => x.GenerateToken(It.Is<User>(u =>
            u.Id == 1
        )), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenUsernameEmpty()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "",
            Email = "test@example.com",
            Password = "password123"
        };

        // Act
        var act = () => _sut.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Username is required.");
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenEmailEmpty()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "",
            Password = "password123"
        };

        // Act
        var act = () => _sut.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Email is required.");
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "existing@example.com",
            Password = "password123"
        };

        _userRepoMock.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(new User { Id = 1, Email = request.Email });

        // Act
        var act = () => _sut.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Email already registered.");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnResponse_WhenCredentialsValid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var user = new User
        {
            Id = 1,
            Name = "testuser",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
        };

        _userRepoMock.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync(user);
        _jwtServiceMock.Setup(x => x.GenerateToken(user)).Returns("jwt-token");

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("jwt-token");
        result.Username.Should().Be(user.Name);
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenEmailNotFound()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "notfound@example.com",
            Password = "password123"
        };

        _userRepoMock.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);

        // Act
        var act = () => _sut.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordInvalid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };

        var user = new User
        {
            Id = 1,
            Name = "testuser",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
        };

        _userRepoMock.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync(user);

        // Act
        var act = () => _sut.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenEmailEmpty()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "",
            Password = "password123"
        };

        // Act
        var act = () => _sut.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Email is required.");
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordEmpty()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = ""
        };

        // Act
        var act = () => _sut.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Password is required.");
    }
}
