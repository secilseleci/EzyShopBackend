using Serilog;
using System.Net;

namespace WebAPI.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _logger;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
        _logger = Log.ForContext<GlobalExceptionMiddleware>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "💥 Global exception yakalandı!");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = "Oops! Something went wrong. Please try again later."
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}