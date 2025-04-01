using AutoMapper;

namespace TaskManagement.Api.Endpoints;

public static class CategoryEndpoint
{
    public static RouteGroupBuilder MapCategoryEndpoints(this RouteGroupBuilder group)
    {
        // Get all categoris
        group.MapGet("/", async (IMapper mapper, IGenericRepository<Category> repository) =>
        {
            var categories = await repository.GetAllAsync();
            if (!categories.Any())
                return Results.NoContent();

            var categoryDtos = mapper.Map<IEnumerable<CategoryDataDto>>(categories);
            return Results.Ok(SuccessResponse<IEnumerable<CategoryDataDto>>.Create(categoryDtos));
        });

        // Get categories by Id
        group.MapGet("/{id:int}", async (IMapper mapper, IGenericRepository<Category> repository, int id) =>
        {
            var category = await repository.GetByIdAsync(id);
            if (category == null)
                return Results.NotFound();
            var categoryDto = mapper.Map<CategoryDataDto>(category);
            return Results.Ok(SuccessResponse<CategoryDataDto>.Create(categoryDto));
        });

        // Create a new categori
        group.MapPost("/", async (IMapper mapper, IGenericRepository<Category> repository, CategoryWriteDto newCategory) =>
        {
            if (newCategory is null)
                return Results.BadRequest();

            var category = mapper.Map<Category>(newCategory);
            await repository.AddAsync(category);

            var categoryDto = mapper.Map<CategoryDataDto>(category);
            return Results.Created($"/api/categories/{category.Id}", SuccessResponse<CategoryDataDto>.Create(categoryDto));
        });


        // Delete a category by Id
        group.MapDelete("/{id:int}", async (IGenericRepository<Category> repository, int id) =>
        {
            var category = await repository.GetByIdAsync(id);
            if (category is null)
                return Results.NotFound();

            await repository.DeleteAsync(id);
            return Results.NoContent();
        });

        return group;
    }
}