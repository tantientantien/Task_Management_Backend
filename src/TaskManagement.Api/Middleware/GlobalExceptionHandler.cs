using Newtonsoft.Json;
using System.Net;
using TaskManagement.Api.Extensions;

namespace TaskManagement.Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, IHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorCode, message) = exception switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, "NOT_FOUND", "The requested resource was not found."),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "UNAUTHORIZED", "You are not authorized to perform this action."),
            ArgumentException => (HttpStatusCode.BadRequest, "BAD_REQUEST", exception.Message),
            _ => (HttpStatusCode.InternalServerError, "INTERNAL_SERVER_ERROR", "An unexpected error occurred.")
        };

        var errorResponse = new ErrorResponse
        {
            ErrorCode = errorCode,
            Message = message,
            Details = _env.IsDevelopment() ? exception.StackTrace : null,
            Timestamp = DateTime.UtcNow
        };
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        var jsonResponse = JsonConvert.SerializeObject(errorResponse, Formatting.Indented);
        return context.Response.WriteAsync(jsonResponse);
    }
}