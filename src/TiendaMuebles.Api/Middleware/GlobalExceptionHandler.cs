using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TiendaMuebles.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            InvalidOperationException => (StatusCodes.Status409Conflict, exception.Message),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, exception.Message),
            FluentValidation.ValidationException => (StatusCodes.Status422UnprocessableEntity, "Error de validacion"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Type = $"/errors/{status}"
        };

        if (exception is FluentValidation.ValidationException valEx)
        {
            problem.Extensions["errors"] = valEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        }

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, ct);
        return true;
    }
}
