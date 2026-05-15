using System.Security.Claims;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechExercise.WebApi.Controllers;
using TechExercise.Application.DTOs.Tasks;
using TechExercise.Application.Interfaces;

namespace TechExercise.Tests.Controllers;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _taskServiceMock;
    private readonly TasksController _sut;

    public TasksControllerTests()
    {
        _taskServiceMock = new Mock<ITaskService>();
        _sut = new TasksController(_taskServiceMock.Object);

        // Setup the HttpContext with a ClaimsPrincipal containing the user ID
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "1")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = principal };
        _sut.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithTasks()
    {
        // Arrange
        var tasks = new List<TaskResponse>
        {
            new() { Id = 1, Title = "Task 1", UserId = 1 },
            new() { Id = 2, Title = "Task 2", UserId = 1 }
        };

        _taskServiceMock.Setup(x => x.GetAllAsync(1)).ReturnsAsync(tasks);

        // Act
        var result = await _sut.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeAssignableTo<IEnumerable<TaskResponse>>().Subject;
        response.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenTaskExists()
    {
        // Arrange
        var task = new TaskResponse { Id = 1, Title = "My Task", UserId = 1 };
        _taskServiceMock.Setup(x => x.GetByIdAsync(1, 1)).ReturnsAsync(task);

        // Act
        var result = await _sut.GetById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<TaskResponse>().Subject;
        response.Id.Should().Be(1);
        response.Title.Should().Be("My Task");
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        _taskServiceMock.Setup(x => x.GetByIdAsync(99, 1)).ReturnsAsync((TaskResponse?)null);

        // Act
        var result = await _sut.GetById(99);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "New Task",
            Description = "Description",
            Status = "pending"
        };

        var createdTask = new TaskResponse
        {
            Id = 42,
            Title = "New Task",
            Description = "Description",
            Status = "pending",
            UserId = 1
        };

        _taskServiceMock.Setup(x => x.CreateAsync(1, request)).ReturnsAsync(createdTask);

        // Act
        var result = await _sut.Create(request);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(TasksController.GetById));
        createdResult.RouteValues!["id"].Should().Be(42);
        createdResult.Value.Should().Be(createdTask);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenTaskUpdated()
    {
        // Arrange
        var request = new UpdateTaskRequest
        {
            Title = "Updated",
            Description = "Updated desc",
            Status = "completed"
        };

        var updatedTask = new TaskResponse
        {
            Id = 1,
            Title = "Updated",
            Description = "Updated desc",
            Status = "completed",
            UserId = 1
        };

        _taskServiceMock.Setup(x => x.UpdateAsync(1, 1, request)).ReturnsAsync(updatedTask);

        // Act
        var result = await _sut.Update(1, request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(updatedTask);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var request = new UpdateTaskRequest { Title = "Ghost", Status = "pending" };
        _taskServiceMock.Setup(x => x.UpdateAsync(99, 1, request)).ReturnsAsync((TaskResponse?)null);

        // Act
        var result = await _sut.Update(99, request);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenDeleted()
    {
        // Arrange
        _taskServiceMock.Setup(x => x.DeleteAsync(1, 1)).ReturnsAsync(true);

        // Act
        var result = await _sut.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        _taskServiceMock.Setup(x => x.DeleteAsync(99, 1)).ReturnsAsync(false);

        // Act
        var result = await _sut.Delete(99);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
