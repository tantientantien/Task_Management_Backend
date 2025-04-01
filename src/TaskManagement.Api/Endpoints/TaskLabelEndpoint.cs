using AutoMapper;

namespace TaskManagement.Api.Endpoints;

public static class TaskLabelEndpoint
{
    public static RouteGroupBuilder MapTaskLabelEndpoints(this RouteGroupBuilder group)
    {
        // Get all task labels 
        group.MapGet("/{taskId:int}", async (IMapper mapper, IGenericRepository<TaskLabel> repository, int taskId) =>
        {
            var taskLabels = await repository.FindAsync(tl => tl.TaskId == taskId, tl => tl.Label);

            if (!taskLabels.Any())
                return Results.NoContent();

            var labelDtos = mapper.Map<IEnumerable<LabelDataDto>>(taskLabels.Select(tl => tl.Label));
            return Results.Ok(SuccessResponse<IEnumerable<LabelDataDto>>.Create(labelDtos));
        });

        // Assign label to task
        group.MapPost("/{taskId:int}", async (IGenericRepository<TaskLabel> repository, int taskId, AssignLabelDto assignLabelDto) =>
        {
            var existingTaskLabel = await repository.FindAsync(tl => tl.TaskId == taskId && tl.LabelId == assignLabelDto.LabelId);
            if (existingTaskLabel.Any())
            {
                return Results.Conflict();
            }
            var taskLabel = new TaskLabel
            {
                TaskId = taskId,
                LabelId = assignLabelDto.LabelId
            };
            await repository.AddAsync(taskLabel);
            return Results.NoContent();
        });


        // Delete label from task
        group.MapDelete("/{taskId:int}/labels/{labelId:int}", async (IGenericRepository<TaskLabel> repository, int taskId, int labelId) =>
        {
            var taskLabels = await repository.FindAsync(tl => tl.TaskId == taskId && tl.LabelId == labelId);
            if (!taskLabels.Any())
            {
                return Results.NotFound();
            }
            await repository.DeleteAsync(taskLabels.First().Id);

            return Results.NoContent();
        });

        return group;
    }
}