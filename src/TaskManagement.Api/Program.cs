using Clerk.Net.AspNetCore.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using TaskManagement.Api.Endpoints;
using TaskManagement.Api.Extensions;
using TaskManagement.Api.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddOpenApi();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddCors();

builder.Services.AddDbContext<TaskManagementContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnectionString")
    )
);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = ClerkAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = ClerkAuthenticationDefaults.AuthenticationScheme;
})
.AddClerkAuthentication(x =>
{
    x.Authority = builder.Configuration["Clerk:Authority"]!;
    x.AuthorizedParty = builder.Configuration["Clerk:AuthorizedParty"]!;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Cookies["__session"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    // options.AddPolicy("AdminAndUser", policy =>
    //     policy.RequireAssertion(context =>
    //         context.User.IsInRole("admin") && context.User.IsInRole("user")));
});





builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IAzureService, AzureService>();
builder.Services.AddScoped<UserManagement>();
builder.Services.AddScoped<UserPermission>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

app.UseGlobalExceptionHandler();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.UseCors(x => x
     .AllowAnyMethod()
     .AllowAnyHeader()
     .AllowCredentials()
     .WithOrigins("http://localhost:3000")
     .SetIsOriginAllowed(origin => true));


app.UseAuthentication();

app.UseAuthorization();


app.MapGroup("/api/users").MapUserEndpoints().RequireAuthorization("AdminOnly").WithTags("User");
app.MapGroup("/api/tasks").MapTaskItemEndpoints().RequireAuthorization().WithTags("Task");
app.MapGroup("/api/categories").MapCategoryEndpoints().RequireAuthorization().WithTags("Category");
app.MapGroup("/api/labels").MapLabelEndpoints().RequireAuthorization().WithTags("Label");
app.MapGroup("/api/tasklabels").MapTaskLabelEndpoints().RequireAuthorization().WithTags("Task Label");
app.MapGroup("/api/comments").MapTaskCommentEndpoints().RequireAuthorization().WithTags("Task Comment");
app.MapGroup("/api/attachments").MapTaskAttachmentEndpoints().RequireAuthorization().WithTags("Attachment");



app.Run();