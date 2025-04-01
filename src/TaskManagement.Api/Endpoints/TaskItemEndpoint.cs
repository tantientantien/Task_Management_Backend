using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TaskManagement.Api.Endpoints;

public static class TaskItemEndpoint
{
    public static RouteGroupBuilder MapTaskItemEndpoints(this RouteGroupBuilder group)
    {
        // Get all tasks
        group.MapGet("/", async (IMapper mapper, IGenericRepository<TaskItem> repository, int pageNumber = 1, int pageSize = 10) =>
        {
            if (pageNumber < 1 || pageSize < 1)
                return Results.BadRequest();

            var skip = (pageNumber - 1) * pageSize;

            var tasks = await repository.Query()
                .Include(task => task.Attachments)
                .Include(task => task.TaskComments)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return tasks.Count == 0
                ? Results.NoContent()
                : Results.Ok(SuccessResponse<IEnumerable<TaskDataDto>>.Create(mapper.Map<IEnumerable<TaskDataDto>>(tasks)));
        });

        // Get task by Id
        group.MapGet("/{id:int}", async (IMapper mapper, IGenericRepository<TaskItem> repository, int id) =>
        {
            var task = await repository.GetByIdAsync(id);
            return task is null
                ? Results.NotFound()
                : Results.Ok(SuccessResponse<TaskDataDto>.Create(mapper.Map<TaskDataDto>(task)));
        });

        // Create a new Task
        group.MapPost("/", async (UserManagement userManagement, IMapper mapper, IGenericRepository<TaskItem> repository, TaskWriteDto newTaskItem) =>
        {
            if (newTaskItem is null)
                return Results.BadRequest();

            var task = mapper.Map<TaskItem>(newTaskItem);
            task.UserId = userManagement.GetCurrentUserId();

            await repository.AddAsync(task);
            return Results.Created($"/api/tasks/{task.Id}", SuccessResponse<TaskDataDto>.Create(mapper.Map<TaskDataDto>(task)));
        });

        // Update a task
        group.MapPatch("/{id:int}", async (UserPermission permission, IMapper mapper, IGenericRepository<TaskItem> repository, int id, TaskPatchDto updateTaskItem) =>
        {
            if (updateTaskItem is null)
                return Results.BadRequest();

            var taskItem = await repository.GetByIdAsync(id);
            if (taskItem is null)
                return Results.NotFound();

            if (!await permission.isAdminOrCreatorOrAssignee(taskItem.UserId, taskItem.AssigneeId))
                return Results.Forbid();

            mapper.Map(updateTaskItem, taskItem);
            await repository.UpdateAsync(taskItem);
            return Results.NoContent();
        });

        // Delete a task by Id
        group.MapDelete("/{id:int}", async (UserPermission permission, IGenericRepository<TaskItem> repository, int id) =>
        {
            var task = await repository.GetByIdAsync(id);
            if (task is null)
                return Results.NotFound();

            if (!await permission.isAdminOrCreator(task.UserId))
                return Results.Forbid();

            await repository.DeleteAsync(id);
            return Results.NoContent();
        });

        return group;
    }
}
