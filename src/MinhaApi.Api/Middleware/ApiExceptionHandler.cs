using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MinhaApi.Api.Middleware;

internal sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Requisição inválida",
                "Os dados informados para a operação são inválidos."),
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Não encontrado",
                "Um dos recursos informados não foi encontrado."),
            InvalidOperationException => (
                StatusCodes.Status409Conflict,
                "Conflito",
                "A operação não pode ser concluída devido a um conflito com os dados atuais."),
            _ => (0, null, null)
        };

        if (statusCode == 0)
        {
            return false;
        }

        logger.LogWarning(exception, "Erro de requisição tratado com status {StatusCode}.", statusCode);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        }, cancellationToken);

        return true;
    }
}
