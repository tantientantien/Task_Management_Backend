using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TaskComment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int TaskId { get; set; }
    [ForeignKey("TaskId")]
    public TaskItem Task { get; set; }
    public string UserId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}