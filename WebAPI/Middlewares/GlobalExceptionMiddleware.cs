using Core.Utilities.Responses;
using Serilog;
using System.Net;

namespace WebAPI.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
    {
        _next = next;
        _env = env;
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
            _logger.Error(ex, "💥 {Path} yolunda bir hata oluştu.", context.Request.Path);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                Message = _env.IsProduction()
                    ? "Oops! Something went wrong. Please try again later."
                    : ex.Message,
                Detail = _env.IsProduction() ? null : ex.StackTrace
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
