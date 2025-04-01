namespace TaskManagement.Api.Endpoints;
public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (UserManagement userManagement) =>
        {
            var users = await userManagement.GetAllUsersAsync();
            if (users == null || !users.Any())
                return Results.NotFound("No users found.");

            var userDtos = users.Select(user => new UserDataDto
            {
                Id = user.Id,
                Email = user.EmailAddresses.FirstOrDefault()?.Email ?? string.Empty,
                UserName = $"{user.FirstName} {user.LastName}".Trim(),
                Avatar = user.ImageUrl,
            });

            return Results.Ok(SuccessResponse<IEnumerable<UserDataDto>>.Create(userDtos));
        });

        return group;
    }
}
