using System.ComponentModel.DataAnnotations;

public class TaskWriteDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title length cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description length cannot exceed 1000 characters.")]
    public string Description { get; set; }

    public bool IsCompleted { get; set; } = false;

    [Required(ErrorMessage = "AssigneeId is required.")]
    public string AssigneeId { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "Due date is required.")]
    [DataType(DataType.DateTime)]
    public DateTime Duedate { get; set; } = DateTime.UtcNow.AddDays(7);

    [Required(ErrorMessage = "CategoryId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a valid positive integer.")]
    public int CategoryId { get; set; }
}
