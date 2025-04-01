public class CommentDataDto
{
    public int Id { get; set; }
    public UserDataDto User { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}