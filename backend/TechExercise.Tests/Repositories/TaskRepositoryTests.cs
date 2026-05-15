using FluentAssertions;
using TechExercise.WebApi.Data;
using TechExercise.WebApi.Models;
using TechExercise.WebApi.Repositories;

namespace TechExercise.Tests.Repositories;

public class TaskRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly UserRepository _userRepo;
    private readonly TaskRepository _repo;

    public TaskRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _userRepo = new UserRepository(fixture.ConnectionFactory);
        _repo = new TaskRepository(fixture.ConnectionFactory);
    }

    private async Task<int> SeedUserAsync()
    {
        var uniqueName = $"task_user_{Guid.NewGuid():N}";
        var user = new User
        {
            Name = uniqueName,
            Email = $"task_test_{Guid.NewGuid()}@example.com",
            PasswordHash = "hash"
        };
        return await _userRepo.CreateAsync(user);
    }

    [Fact]
    public async Task CreateAsync_ShouldInsertTaskAndReturnId()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var userId = await SeedUserAsync();

        var task = new TaskItem
        {
            Title = "Integration Test Task",
            Description = "Created during testing",
            Status = "pending",
            UserId = userId
        };

        // Act
        var id = await _repo.CreateAsync(task);

        // Assert
        id.Should().BeGreaterThan(0);

        var saved = await _repo.GetByIdAsync(id);
        saved.Should().NotBeNull();
        saved!.Title.Should().Be("Integration Test Task");
        saved.Description.Should().Be("Created during testing");
        saved.Status.Should().Be("pending");
        saved.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task CreateAsync_ShouldInsertTaskWithAllFields()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var userId = await SeedUserAsync();
        var dueDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);

        var task = new TaskItem
        {
            Title = "Task with due date",
            Description = "Has description and due date",
            Status = "in_progress",
            DueDate = dueDate,
            UserId = userId
        };

        // Act
        var id = await _repo.CreateAsync(task);
        var saved = await _repo.GetByIdAsync(id);

        // Assert
        saved.Should().NotBeNull();
        saved!.Title.Should().Be("Task with due date");
        saved.Description.Should().Be("Has description and due date");
        saved.Status.Should().Be("in_progress");
        saved.DueDate.Should().Be(dueDate);
        saved.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task CreateAsync_ShouldAllowNullDescriptionAndDueDate()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var userId = await SeedUserAsync();

        var task = new TaskItem
        {
            Title = "Minimal Task",
            Status = "pending",
            UserId = userId
        };

        // Act
        var id = await _repo.CreateAsync(task);
        var saved = await _repo.GetByIdAsync(id);

        // Assert
        saved.Should().NotBeNull();
        saved!.Description.Should().BeNull();
        saved.DueDate.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask_WhenExists()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var userId = await SeedUserAsync();

        var task = new TaskItem { Title = "Findable Task", Status = "pending", UserId = userId };
        var id = await _repo.CreateAsync(task);

        // Act
        var result = await _repo.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Title.Should().Be("Findable Task");
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
    public async Task GetByUserIdAsync_ShouldReturnAllTasksForUser()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var userId = await SeedUserAsync();
        var anotherUserId = await SeedUserAsync();

        await _repo.CreateAsync(new TaskItem { Title = "Task A", Status = "pending", UserId = userId });
        await _repo.CreateAsync(new TaskItem { Title = "Task B", Status = "in_progress", UserId = userId });
        await _repo.CreateAsync(new TaskItem { Title = "Task C", Status = "completed", UserId = userId });
        // Other user's task - should NOT be returned
        await _repo.CreateAsync(new TaskItem { Title = "Other User Task", Status = "pending", UserId = anotherUserId });

        // Act
        var tasks = await _repo.GetByUserIdAsync(userId);

        // Assert
        tasks.Should().HaveCount(3);
        tasks.Select(t => t.Title).Should().Contain(["Task A", "Task B", "Task C"]);
        tasks.Select(t => t.Title).Should().NotContain("Other User Task");
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnTasksOrderedByCreatedAtDesc()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var userId = await SeedUserAsync();

        var id1 = await _repo.CreateAsync(new TaskItem { Title = "First", Status = "pending", UserId = userId });
        await Task.Delay(10);
        var id2 = await _repo.CreateAsync(new TaskItem { Title = "Second", Status = "pending", UserId = userId });
        await Task.Delay(10);
        var id3 = await _repo.CreateAsync(new TaskItem { Title = "Third", Status = "pending", UserId = userId });

        // Act
        var tasks = (await _repo.GetByUserIdAsync(userId)).ToList();

        // Assert - newest first
        tasks[0].Title.Should().Be("Third");
        tasks[1].Title.Should().Be("Second");
        tasks[2].Title.Should().Be("First");
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnEmpty_WhenNoTasks()
    {
        // Arrange
        await _fixture.ResetTablesAsync();

        // Act
        var tasks = await _repo.GetByUserIdAsync(9999);

        // Assert
        tasks.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAllFieldsAndReturnTrue()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var userId = await SeedUserAsync();

        var id = await _repo.CreateAsync(new TaskItem
        {
            Title = "Original Title",
            Description = "Original Description",
            Status = "pending",
            UserId = userId
        });

        var saved = await _repo.GetByIdAsync(id);
        var dueDate = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc);

        saved!.Title = "Updated Title";
        saved.Description = "Updated Description";
        saved.Status = "completed";
        saved.DueDate = dueDate;

        // Act
        var result = await _repo.UpdateAsync(saved);
        var updated = await _repo.GetByIdAsync(id);

        // Assert
        result.Should().BeTrue();
        updated.Should().NotBeNull();
        updated!.Title.Should().Be("Updated Title");
        updated.Description.Should().Be("Updated Description");
        updated.Status.Should().Be("completed");
        updated.DueDate.Should().Be(dueDate);
        updated.UpdatedAt.Should().BeAfter(saved.UpdatedAt);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenTaskNotExists()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = 9999,
            Title = "Ghost",
            Description = null,
            Status = "pending",
            UserId = 1
        };

        // Act
        var result = await _repo.UpdateAsync(task);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_ShouldNotUpdate_WhenUserIdDiffers()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var userId1 = await SeedUserAsync();
        var userId2 = await SeedUserAsync();

        var id = await _repo.CreateAsync(new TaskItem
        {
            Title = "User 1 Task",
            Status = "pending",
            UserId = userId1
        });

        var task = await _repo.GetByIdAsync(id);
        task!.UserId = userId2; // Try to hijack
        task.Title = "Hijacked";

        // Act
        var result = await _repo.UpdateAsync(task);

        // Assert
        result.Should().BeFalse();

        var unchanged = await _repo.GetByIdAsync(id);
        unchanged!.Title.Should().Be("User 1 Task");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var userId = await SeedUserAsync();

        var id = await _repo.CreateAsync(new TaskItem
        {
            Title = "Deletable Task",
            Status = "pending",
            UserId = userId
        });

        // Act
        var result = await _repo.DeleteAsync(id, userId);
        var deleted = await _repo.GetByIdAsync(id);

        // Assert
        result.Should().BeTrue();
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var userId = await SeedUserAsync();

        // Act
        var result = await _repo.DeleteAsync(9999, userId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotDeleteWithWrongUserId()
    {
        // Arrange
        await _fixture.ResetTablesAsync();
        var userId = await SeedUserAsync();

        var id = await _repo.CreateAsync(new TaskItem
        {
            Title = "Protected Task",
            Status = "pending",
            UserId = userId
        });

        // Act - try to delete with a different user id
        var result = await _repo.DeleteAsync(id, userId + 9999);

        // Assert
        result.Should().BeFalse();

        var remaining = await _repo.GetByIdAsync(id);
        remaining.Should().NotBeNull();
        remaining!.Title.Should().Be("Protected Task");
    }
}
