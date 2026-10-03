using System.Net;
using System.Text.Json;
using WebcamStudio.Application.Common;

namespace WebcamStudio.Api.Middleware;


public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepcion no manejada procesando {Method} {Path}", context.Request.Method, context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errors = _environment.IsDevelopment()
                ? new List<string> { ex.Message, ex.StackTrace ?? string.Empty }
                : new List<string>();

            var response = ApiResponse<object>.Fail("Ocurrio un error inesperado procesando la solicitud.", errors);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
