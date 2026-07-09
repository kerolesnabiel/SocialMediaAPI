using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SocialMediaDomain.Exceptions;

namespace SocialMediaAPI.Handlers;

public sealed class GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService,
        IHostEnvironment env ) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}",
            httpContext.TraceIdentifier);

        (int statusCode, string title) = MapException(exception);

        ProblemDetails problemDetails = new()
        {
            Title = title,
            Status = statusCode,
            Type = GetProblemType(statusCode),
            Detail = GetSafeErrorMessage(exception)
        };

        if (exception is ValidationException ex)
            problemDetails.Extensions["Errors"] = ex.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage));

        httpContext.Response.StatusCode = statusCode;
        return await problemDetailsService.TryWriteAsync(new()
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private static (int statusCode, string title) MapException(Exception exception) => exception switch
    {
        AppException appEx => ((int)appEx.StatusCode, appEx.Message),
        ValidationException => (StatusCodes.Status400BadRequest, "One or more validation errors occurred"),
        ArgumentNullException => (StatusCodes.Status400BadRequest, "Invalid argument provided"),
        ArgumentException => (StatusCodes.Status400BadRequest, "Invalid argument provided"),
        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
        _ => (StatusCodes.Status500InternalServerError, "An Unexpected error occurred")
    };

    private static string GetProblemType(int statusCode) => statusCode switch
    {
        400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        401 => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
        403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
        404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1"
    };

    private string? GetSafeErrorMessage(Exception exception)
    {
        if (env.IsDevelopment() || exception is AppException)
            return exception.Message;

        return null;
    }
}
