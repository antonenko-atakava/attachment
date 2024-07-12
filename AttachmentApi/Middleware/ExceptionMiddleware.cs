using System.Net;
using Newtonsoft.Json;

namespace AttachmentApi.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Error Exception Middleware]: {ex}");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        int statusCode;
        string message;

        switch (ex)
        {
            case ArgumentException _:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "Неправильный запрос.";
                break;
            case UnauthorizedAccessException _:
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = "Нет доступа.";
                break;
            case KeyNotFoundException _:
                statusCode = (int)HttpStatusCode.NotFound;
                message = "Не найдено.";
                break;
            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "Внутренняя ошибка сервера.";
                break;
        }

        context.Response.StatusCode = statusCode;

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = message,
            Detailed = ex.Message
        };

        var jsonResponse = JsonConvert.SerializeObject(response);
        return context.Response.WriteAsync(jsonResponse);
    }
}