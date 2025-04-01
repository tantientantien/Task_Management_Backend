using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

public class UserPermission
{
    private readonly UserManagement _userManagement;

    public UserPermission(UserManagement userManagement)
    {
        _userManagement = userManagement;
    }

    public async Task<bool> isAdminOrCreator(string userId)
    {
        var (currentUser, userRoles) = await GetCurrentUserAndRolesAsync();
        if (currentUser == null || string.IsNullOrEmpty(userId)) return false;

        return userRoles.Contains("admin", StringComparer.OrdinalIgnoreCase) || currentUser.Id == userId;
    }

    public async Task<bool> isAdminOrCreatorOrAssignee(string userId, string assigneeId)
    {
        var (currentUser, userRoles) = await GetCurrentUserAndRolesAsync();

        if (currentUser == null) return false;

        return userRoles.Contains("admin", StringComparer.OrdinalIgnoreCase) ||
               currentUser.Id == userId ||
               currentUser.Id == assigneeId;
    }

    private async Task<(ClerkUser, HashSet<string>)> GetCurrentUserAndRolesAsync()
    {
        var currentUser = await _userManagement.GetCurrentUserAsync();
        if (currentUser == null) return (null, new HashSet<string>());

        var userRoles = _userManagement.GetCurrentUserRoles()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return (currentUser, userRoles);
    }
}