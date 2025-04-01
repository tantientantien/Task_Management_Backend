using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ClerkUser
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("email_addresses")]
    public List<EmailAddress> EmailAddresses { get; set; } = new();

    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }
}

public class EmailAddress
{
    [JsonPropertyName("email_address")]
    public string Email { get; set; } = string.Empty;
}

public class UserManagement
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;
    private readonly string _clerkApiKey;

    public UserManagement(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClient;
        _clerkApiKey = configuration["Clerk:ApiKey"];
    }

    public string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public async Task<ClerkUser> GetCurrentUserAsync()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId)) return null;

        return await FetchClerkUserAsync(userId);
    }

    public async Task<List<ClerkUser>> GetAllUsersAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clerk.com/v1/users");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _clerkApiKey);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) return new List<ClerkUser>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ClerkUser>>(json) ?? new List<ClerkUser>();
    }

    public async Task<ClerkUser> FetchClerkUserAsync(string userId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.clerk.com/v1/users/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _clerkApiKey);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ClerkUser>(json);
    }

    public IEnumerable<string> GetCurrentUserRoles()
    {
        var userClaims = _httpContextAccessor.HttpContext?.User;
        if (userClaims == null)
            return new List<string>();

        var roles = userClaims.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "roles")
            .SelectMany(c => c.Value.Split(','))
            .ToList();

        return roles;
    }
}
