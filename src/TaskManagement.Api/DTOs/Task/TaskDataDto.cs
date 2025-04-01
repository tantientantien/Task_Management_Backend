public class TaskDataDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public string UserId { get; set; }
    public string AssigneeId { get; set; }
    public int AttachmentCount { get; set; }
    public int CommentCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CategoryId { get; set; }
    public DateTime Duedate { get; set; }
}


// public int AttachmentCount { get; set; }
// public int CommentCount { get; set; }
//public List<TaskLabelDataDto> labels { get; set; } = new();
//public UserDataDto Assignee { get; set; } = new();