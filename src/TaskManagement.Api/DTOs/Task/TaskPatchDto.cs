using System.ComponentModel.DataAnnotations;


public class TaskPatchDto
{
    [StringLength(200, ErrorMessage = "Title length cannot exceed 200 characters.")]
    public string Title { get; set; }

    [StringLength(1000, ErrorMessage = "Description length cannot exceed 1000 characters.")]
    public string Description { get; set; }
    public bool? IsCompleted { get; set; }

    public string AssigneeId { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? Duedate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a valid positive integer.")]
    public int? CategoryId { get; set; }
}
