using AutoMapper;

namespace TaskManagement.Api.Endpoints;

public static class LabelEndpoint
{
    public static RouteGroupBuilder MapLabelEndpoints(this RouteGroupBuilder group)
    {
        // Get all labels
        group.MapGet("/", async (IMapper mapper, IGenericRepository<Label> repository) =>
        {
            var labels = await repository.GetAllAsync();
            if (!labels.Any())
                return Results.NoContent();

            var labelDtos = mapper.Map<IEnumerable<LabelDataDto>>(labels);
            return Results.Ok(SuccessResponse<IEnumerable<LabelDataDto>>.Create(labelDtos));
        });

        // Get label by Id
        group.MapGet("/{id:int}", async (IMapper mapper, IGenericRepository<Label> repository, int id) =>
        {
            var label = await repository.GetByIdAsync(id);
            if (label == null)
                return Results.NotFound();
            var labelDto = mapper.Map<LabelDataDto>(label);
            return Results.Ok(SuccessResponse<LabelDataDto>.Create(labelDto));
        });

        // Create a new label
        group.MapPost("/", async (IMapper mapper, IGenericRepository<Label> repository, LabelWriteDto newLabel) =>
        {
            if (newLabel is null)
                return Results.BadRequest();

            var Label = mapper.Map<Label>(newLabel);
            await repository.AddAsync(Label);

            var LabelDto = mapper.Map<LabelDataDto>(Label);
            return Results.Created($"/api/labels/{Label.Id}", SuccessResponse<LabelDataDto>.Create(LabelDto));
        });


        // Delete a label by Id
        group.MapDelete("/{id:int}", async (IGenericRepository<Label> repository, int id) =>
        {
            var label = await repository.GetByIdAsync(id);
            if (label is null)
                return Results.NotFound();

            await repository.DeleteAsync(id);
            return Results.NoContent();
        });

        return group;
    }
}