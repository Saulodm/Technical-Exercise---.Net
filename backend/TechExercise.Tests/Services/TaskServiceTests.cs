using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using TechExercise.WebApi.DTOs.Tasks;
using TechExercise.WebApi.Models;
using TechExercise.WebApi.Repositories;
using TechExercise.WebApi.Services;

namespace TechExercise.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock;
    private readonly Mock<ILogger<TaskService>> _loggerMock;
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        _taskRepoMock = new Mock<ITaskRepository>();
        _loggerMock = new Mock<ILogger<TaskService>>();
        _sut = new TaskService(_taskRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTasksForUser()
    {
        // Arrange
        var userId = 1;
        var tasks = new List<TaskItem>
        {
            new() { Id = 1, Title = "Task 1", UserId = userId },
            new() { Id = 2, Title = "Task 2", UserId = userId }
        };

        _taskRepoMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(tasks);

        // Act
        var result = await _sut.GetAllAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Title == "Task 1");
        _taskRepoMock.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoTasks()
    {
        // Arrange
        _taskRepoMock.Setup(x => x.GetByUserIdAsync(1)).ReturnsAsync(new List<TaskItem>());

        // Act
        var result = await _sut.GetAllAsync(1);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask_WhenOwnedByUser()
    {
        // Arrange
        var taskId = 1;
        var userId = 1;
        var task = new TaskItem { Id = taskId, Title = "My Task", UserId = userId };

        _taskRepoMock.Setup(x => x.GetByIdAsync(taskId)).ReturnsAsync(task);

        // Act
        var result = await _sut.GetByIdAsync(taskId, userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskId);
        result.Title.Should().Be("My Task");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotOwnedByUser()
    {
        // Arrange
        var taskId = 1;
        var task = new TaskItem { Id = taskId, Title = "Someone Else's Task", UserId = 2 };

        _taskRepoMock.Setup(x => x.GetByIdAsync(taskId)).ReturnsAsync(task);

        // Act
        var result = await _sut.GetByIdAsync(taskId, userId: 1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        // Arrange
        _taskRepoMock.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _sut.GetByIdAsync(99, 1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAndReturnTask()
    {
        // Arrange
        var userId = 1;
        var request = new CreateTaskRequest
        {
            Title = "New Task",
            Description = "Description",
            Status = "pending",
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        _taskRepoMock.Setup(x => x.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(42);

        // Act
        var result = await _sut.CreateAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(42);
        result.Title.Should().Be(request.Title);
        result.Description.Should().Be(request.Description);
        result.Status.Should().Be(request.Status);
        result.UserId.Should().Be(userId);

        _taskRepoMock.Verify(x => x.CreateAsync(It.Is<TaskItem>(t =>
            t.Title == request.Title &&
            t.UserId == userId
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAndReturnTask_WhenOwnedByUser()
    {
        // Arrange
        var taskId = 1;
        var userId = 1;
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Old Title",
            Description = "Old Description",
            Status = "pending",
            UserId = userId
        };

        var request = new UpdateTaskRequest
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Status = "in_progress",
            DueDate = DateTime.UtcNow.AddDays(3)
        };

        _taskRepoMock.Setup(x => x.GetByIdAsync(taskId)).ReturnsAsync(existingTask);
        _taskRepoMock.Setup(x => x.UpdateAsync(It.IsAny<TaskItem>())).ReturnsAsync(true);

        // Act
        var result = await _sut.UpdateAsync(taskId, userId, request);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be(request.Title);
        result.Description.Should().Be(request.Description);
        result.Status.Should().Be(request.Status);

        _taskRepoMock.Verify(x => x.UpdateAsync(It.Is<TaskItem>(t =>
            t.Id == taskId &&
            t.Title == request.Title &&
            t.UserId == userId
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotOwnedByUser()
    {
        // Arrange
        var taskId = 1;
        var task = new TaskItem { Id = taskId, Title = "Task", UserId = 2 };

        _taskRepoMock.Setup(x => x.GetByIdAsync(taskId)).ReturnsAsync(task);

        // Act
        var result = await _sut.UpdateAsync(taskId, userId: 1, new UpdateTaskRequest
        {
            Title = "Hacked",
            Status = "completed"
        });

        // Assert
        result.Should().BeNull();
        _taskRepoMock.Verify(x => x.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        // Arrange
        _taskRepoMock.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _sut.UpdateAsync(99, 1, new UpdateTaskRequest
        {
            Title = "Ghost",
            Status = "pending"
        });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenOwnedByUser()
    {
        // Arrange
        var taskId = 1;
        var userId = 1;
        var task = new TaskItem { Id = taskId, UserId = userId };

        _taskRepoMock.Setup(x => x.GetByIdAsync(taskId)).ReturnsAsync(task);
        _taskRepoMock.Setup(x => x.DeleteAsync(taskId)).ReturnsAsync(true);

        // Act
        var result = await _sut.DeleteAsync(taskId, userId);

        // Assert
        result.Should().BeTrue();
        _taskRepoMock.Verify(x => x.DeleteAsync(taskId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotOwnedByUser()
    {
        // Arrange
        var taskId = 1;
        var task = new TaskItem { Id = taskId, UserId = 2 };

        _taskRepoMock.Setup(x => x.GetByIdAsync(taskId)).ReturnsAsync(task);

        // Act
        var result = await _sut.DeleteAsync(taskId, userId: 1);

        // Assert
        result.Should().BeFalse();
        _taskRepoMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        // Arrange
        _taskRepoMock.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _sut.DeleteAsync(99, 1);

        // Assert
        result.Should().BeFalse();
    }
}
