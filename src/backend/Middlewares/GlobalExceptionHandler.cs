using CajuAjuda.Backend.Exceptions;
using System.Net;
using System.Text.Json;

namespace CajuAjuda.Backend.Middlewares;

public class GlobalExceptionHandler : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode;
        string message;

        switch (exception)
        {
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                _logger.LogInformation(exception, "Recurso não encontrado: {Message}", message);
                break;
            case BusinessRuleException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                _logger.LogWarning(exception, "Regra de negócio violada: {Message}", message);
                break;
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Forbidden;
                message = "Você não tem permissão para realizar esta ação.";
                _logger.LogWarning(exception, "Tentativa de acesso não autorizado: {Message}", exception.Message);
                break;
            default:
                statusCode = HttpStatusCode.InternalServerError;
                message = "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde.";
                // Logamos o erro completo com stack trace para depuração
                _logger.LogError(exception, "Um erro inesperado ocorreu: {Message}", exception.Message);
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        
        var errorResponse = new { error = message };
        var jsonResponse = JsonSerializer.Serialize(errorResponse);
        
        return context.Response.WriteAsync(jsonResponse);
    }
}