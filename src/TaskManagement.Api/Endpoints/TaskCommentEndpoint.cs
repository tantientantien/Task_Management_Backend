using AutoMapper;

namespace TaskManagement.Api.Endpoints;

public static class TaskCommentEndpoint
{
    public static RouteGroupBuilder MapTaskCommentEndpoints(this RouteGroupBuilder group)
    {
        // Get all comments by taskId
        group.MapGet("/{taskId:int}", async (IMapper mapper, IGenericRepository<TaskComment> repository, UserManagement userManagement, int taskId) =>
        {
            var taskComments = await repository.FindAsync(tc => tc.TaskId == taskId);
            if (!taskComments.Any())
                return Results.NoContent();
            var commentDtos = mapper.Map<List<CommentDataDto>>(taskComments);

            foreach (var commentDto in commentDtos)
            {
                var user = await userManagement.FetchClerkUserAsync(commentDto.User.Id);
                if (user != null)
                {
                    commentDto.User = new UserDataDto
                    {
                        Id = user.Id,
                        Email = user.EmailAddresses.FirstOrDefault()?.Email ?? string.Empty,
                        UserName = $"{user.FirstName} {user.LastName}".Trim(),
                        Avatar = user.ImageUrl
                    };
                }
                else
                {
                    commentDto.User = null;
                }
            }

            return Results.Ok(SuccessResponse<IEnumerable<CommentDataDto>>.Create(commentDtos));
        });

        // Add a comment to a task
        group.MapPost("/{taskId:int}", async (UserManagement userManagement, IMapper mapper, IGenericRepository<TaskComment> repository, int taskId, CommentWriteDto commentDto) =>
        {
            if (commentDto is null || string.IsNullOrWhiteSpace(commentDto.Content))
                return Results.BadRequest();

            var taskComment = mapper.Map<TaskComment>(commentDto);
            taskComment.UserId = userManagement.GetCurrentUserId();
            taskComment.TaskId = taskId;
            taskComment.CreatedAt = DateTime.UtcNow;

            await repository.AddAsync(taskComment);
            return Results.NoContent();
        });

        // Delete a comment
        group.MapDelete("/{commentId:int}", async (UserPermission permission, IGenericRepository<TaskComment> repository, int commentId) =>
        {
            var comment = await repository.GetByIdAsync(commentId);
            if (comment is null)
                return Results.NotFound();

            if (!await permission.isAdminOrCreator(comment.UserId))
                return Results.Forbid();

            await repository.DeleteAsync(commentId);
            return Results.NoContent();
        });

        return group;
    }
}
