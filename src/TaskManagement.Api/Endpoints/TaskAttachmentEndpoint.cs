using AutoMapper;

namespace TaskManagement.Api.Endpoints;

public static class TaskAttachmentEndpoint
{
    public static RouteGroupBuilder MapTaskAttachmentEndpoints(this RouteGroupBuilder group)
    {
        // Get all attachemnt
        group.MapGet("/{taskId:int}", async (IMapper mapper, IGenericRepository<TaskAttachment> repository, IAzureService azureService, int taskId) =>
        {
            var attachments = await repository.FindAsync(at => at.TaskId == taskId);
            if (!attachments.Any())
                return Results.NoContent();

            var attachmentDtos = mapper.Map<IEnumerable<AttachmentDataDto>>(attachments);
            return Results.Ok(SuccessResponse<IEnumerable<AttachmentDataDto>>.Create(attachmentDtos));
        });


        // Upload an attachment
        group.MapPost("/{taskId:int}", async (
            IMapper mapper,
            IGenericRepository<TaskAttachment> attachmentRepository,
            IGenericRepository<TaskItem> taskRepository,
            IAzureService azureService,
            UserPermission permission,
            int taskId,
            IFormFile file) =>
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest(SuccessResponse<string>.Create(null));

            var task = await taskRepository.GetByIdAsync(taskId);
            if (task == null)
                return Results.NotFound(SuccessResponse<string>.Create(null));
            
            if (!await permission.isAdminOrCreatorOrAssignee(task.UserId, task.AssigneeId))
                return Results.Forbid();

            try
            {
                var uploadResult = await azureService.UploadAsync(file);
                var taskAttachment = new TaskAttachment
                {
                    TaskId = taskId,
                    FileName = uploadResult.FileName,
                    FileUrl = uploadResult.Url,
                    UploadedAt = uploadResult.UploadDate
                };
                await attachmentRepository.AddAsync(taskAttachment);
                var attachmentDto = mapper.Map<AttachmentDataDto>(taskAttachment);
                return Results.Ok(SuccessResponse<AttachmentDataDto>.Create(attachmentDto));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(SuccessResponse<string>.Create(null, ex.Message));
            }
            catch
            {
                return Results.Problem();
            }
        }).Accepts<IFormFile>("multipart/form-data").DisableAntiforgery();


        // Delete an attachemnt
        group.MapDelete("/{taskId:int}/{attachmentId:int}", async (
            IGenericRepository<TaskAttachment> attachmentRepository,
            IGenericRepository<TaskItem> taskRepository,
            IAzureService azureService,
            UserPermission permission,
            int taskId,
            int attachmentId) =>
        {
            var task = await taskRepository.GetByIdAsync(taskId);
            if (task == null)
                return Results.NotFound(SuccessResponse<string>.Create(null));
            
            if (!await permission.isAdminOrCreatorOrAssignee(task.UserId, task.AssigneeId))
                return Results.Forbid();

            var attachment = await attachmentRepository.GetByIdAsync(attachmentId);
            if (attachment == null || attachment.TaskId != taskId)
                return Results.NotFound(SuccessResponse<string>.Create(null));

            try
            {
                var deletedFromAzure = await azureService.DeleteAsync(attachment.FileName);
                await attachmentRepository.DeleteAsync(attachmentId);
                return Results.NoContent();
            }
            catch
            {
                return Results.Problem();
            }
        }).DisableAntiforgery();


        return group;
    }
}