using System.ComponentModel.DataAnnotations;

namespace TechExercise.WebApi.DTOs.Tasks;

public class CreateTaskRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(300, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 300 characters")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [RegularExpression("^(pending|in_progress|completed)$", ErrorMessage = "Status must be 'pending', 'in_progress', or 'completed'")]
    public string Status { get; set; } = "pending";

    public DateTime? DueDate { get; set; }
}
