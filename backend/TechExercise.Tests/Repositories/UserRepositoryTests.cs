using FluentAssertions;
using TechExercise.Domain.Entities;
using TechExercise.Infrastructure.Data;
using TechExercise.Infrastructure.Repositories;

namespace TechExercise.Tests.Repositories;

public class UserRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly UserRepository _repo;

    public UserRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repo = new UserRepository(fixture.ConnectionFactory);
    }

    [Fact]
    public async Task CreateAsync_ShouldInsertUserAndReturnId()
    {
        // Arrange
        var user = new User
        {
            Name = "testuser",
            Email = "testuser@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
        };

        // Act
        var id = await _repo.CreateAsync(user);

        // Assert
        id.Should().BeGreaterThan(0);

        // Verify the user was actually inserted
        var saved = await _repo.GetByIdAsync(id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("testuser");
        saved.Email.Should().Be("testuser@example.com");
    }

    [Fact]
    public async Task CreateAsync_ShouldUseNameAsUsername()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var user = new User
        {
            Name = "unique_user_42",
            Email = "unique42@example.com",
            PasswordHash = "hash"
        };

        // Act
        var id = await _repo.CreateAsync(user);

        // Assert - verify via GetByUsername that Name maps to username column
        var byUsername = await _repo.GetByUsernameAsync("unique_user_42");
        byUsername.Should().NotBeNull();
        byUsername!.Id.Should().Be(id);
        byUsername.Name.Should().Be("unique_user_42");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var user = new User
        {
            Name = "getbyid_user",
            Email = "getbyid@example.com",
            PasswordHash = "hash"
        };
        var id = await _repo.CreateAsync(user);

        // Act
        var result = await _repo.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be("getbyid_user");
        result.Email.Should().Be("getbyid@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repo.GetByIdAsync(9999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenExists()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var user = new User
        {
            Name = "email_user",
            Email = "email_lookup@example.com",
            PasswordHash = "hash"
        };
        var id = await _repo.CreateAsync(user);

        // Act
        var result = await _repo.GetByEmailAsync("email_lookup@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Email.Should().Be("email_lookup@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repo.GetByEmailAsync("nonexistent@example.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnUser_WhenExists()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var user = new User
        {
            Name = "username_lookup",
            Email = "username_test@example.com",
            PasswordHash = "hash"
        };
        var id = await _repo.CreateAsync(user);

        // Act
        var result = await _repo.GetByUsernameAsync("username_lookup");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be("username_lookup");
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repo.GetByUsernameAsync("unknown_user");

        // Assert
        result.Should().BeNull();
    }
}
