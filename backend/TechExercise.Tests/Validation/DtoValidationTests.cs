using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using TechExercise.WebApi.DTOs.Auth;
using TechExercise.WebApi.DTOs.Tasks;

namespace TechExercise.Tests.Validation;

public class DtoValidationTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    // ───── RegisterRequest ─────

    [Fact]
    public void RegisterRequest_ShouldBeValid_WhenAllFieldsValid()
    {
        var dto = new RegisterRequest
        {
            Username = "validuser",
            Email = "user@example.com",
            Password = "password123"
        };

        var errors = ValidateModel(dto);
        errors.Should().BeEmpty();
    }

    [Fact]
    public void RegisterRequest_ShouldBeInvalid_WhenUsernameMissing()
    {
        var dto = new RegisterRequest
        {
            Username = "",
            Email = "user@example.com",
            Password = "password123"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("Username"));
    }

    [Fact]
    public void RegisterRequest_ShouldBeInvalid_WhenUsernameTooShort()
    {
        var dto = new RegisterRequest
        {
            Username = "ab",
            Email = "user@example.com",
            Password = "password123"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("Username"));
    }

    [Fact]
    public void RegisterRequest_ShouldBeInvalid_WhenEmailInvalid()
    {
        var dto = new RegisterRequest
        {
            Username = "validuser",
            Email = "not-an-email",
            Password = "password123"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("Email"));
    }

    [Fact]
    public void RegisterRequest_ShouldBeInvalid_WhenPasswordTooShort()
    {
        var dto = new RegisterRequest
        {
            Username = "validuser",
            Email = "user@example.com",
            Password = "12345"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("Password"));
    }

    // ───── LoginRequest ─────

    [Fact]
    public void LoginRequest_ShouldBeValid_WhenAllFieldsValid()
    {
        var dto = new LoginRequest
        {
            Email = "user@example.com",
            Password = "password123"
        };

        var errors = ValidateModel(dto);
        errors.Should().BeEmpty();
    }

    [Fact]
    public void LoginRequest_ShouldBeInvalid_WhenEmailMissing()
    {
        var dto = new LoginRequest
        {
            Email = "",
            Password = "password123"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("Email"));
    }

    [Fact]
    public void LoginRequest_ShouldBeInvalid_WhenPasswordMissing()
    {
        var dto = new LoginRequest
        {
            Email = "user@example.com",
            Password = ""
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("Password"));
    }

    // ───── CreateTaskRequest ─────

    [Fact]
    public void CreateTaskRequest_ShouldBeValid_WhenAllFieldsValid()
    {
        var dto = new CreateTaskRequest
        {
            Title = "My Task",
            Description = "Description",
            Status = "in_progress",
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        var errors = ValidateModel(dto);
        errors.Should().BeEmpty();
    }

    [Fact]
    public void CreateTaskRequest_ShouldBeValid_WithOnlyRequiredFields()
    {
        var dto = new CreateTaskRequest
        {
            Title = "Minimal Task",
            Status = "pending"
        };

        var errors = ValidateModel(dto);
        errors.Should().BeEmpty();
    }

    [Fact]
    public void CreateTaskRequest_ShouldBeInvalid_WhenTitleMissing()
    {
        var dto = new CreateTaskRequest
        {
            Title = "",
            Status = "pending"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("Title"));
    }

    [Fact]
    public void CreateTaskRequest_ShouldBeInvalid_WhenStatusInvalid()
    {
        var dto = new CreateTaskRequest
        {
            Title = "Task",
            Status = "invalid_status"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("Status"));
    }

    // ───── UpdateTaskRequest ─────

    [Fact]
    public void UpdateTaskRequest_ShouldBeValid_WhenAllFieldsValid()
    {
        var dto = new UpdateTaskRequest
        {
            Title = "Updated Task",
            Description = "Updated",
            Status = "completed"
        };

        var errors = ValidateModel(dto);
        errors.Should().BeEmpty();
    }

    [Fact]
    public void UpdateTaskRequest_ShouldBeInvalid_WhenStatusInvalid()
    {
        var dto = new UpdateTaskRequest
        {
            Title = "Task",
            Status = "bad_status"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("Status"));
    }

    [Fact]
    public void UpdateTaskRequest_ShouldBeInvalid_WhenTitleMissing()
    {
        var dto = new UpdateTaskRequest
        {
            Title = "",
            Status = "pending"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("Title"));
    }
}
