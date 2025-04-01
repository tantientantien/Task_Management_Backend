using System.ComponentModel.DataAnnotations;

public class CommentWriteDto
{
    [Required]
    public string Content { get; set; }
}