using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SessionLogger.Exceptions;

public class ExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ProblemException problemException => GetProblemDetails(problemException),
            ValidationException validationException => GetProblemDetails(validationException),
            UnauthorizedAccessException unauthorizedAccessException => GetProblemDetails(unauthorizedAccessException),
            ForbiddenAccessException forbiddenAccessException => GetProblemDetails(forbiddenAccessException),
            NotFoundException notFoundException => GetProblemDetails(notFoundException),
            _ => GetProblemDetails(exception)
        };
        
        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }

    private static ProblemDetails GetProblemDetails(ProblemException problemException) => new()
    {
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        Status = StatusCodes.Status400BadRequest,
        Title = problemException.Title,
        Detail = problemException.Message,
    };
    
    private static ValidationProblemDetails GetProblemDetails(ValidationException validationException) => new(GetValidationErrors(validationException))
    {
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        Status = StatusCodes.Status400BadRequest,
        Title = validationException.GetType().Name,
        Detail = validationException.Message,
    };
    
    private static Dictionary<string, string[]> GetValidationErrors(ValidationException validationException) => validationException.Errors
        .ToDictionary(x => x.PropertyName, x => new[] { x.ErrorMessage });

    private static ProblemDetails GetProblemDetails(UnauthorizedAccessException unauthorizedAccessException) => new()
    {
        Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
        Status = StatusCodes.Status401Unauthorized,
        Title = unauthorizedAccessException.GetType().Name,
        Detail = unauthorizedAccessException.Message,
    };
    
    private static ProblemDetails GetProblemDetails(ForbiddenAccessException forbiddenAccessException) => new()
    {
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
        Status = StatusCodes.Status403Forbidden,
        Title = forbiddenAccessException.GetType().Name,
        Detail = forbiddenAccessException.Message,
    };
    
    private static ProblemDetails GetProblemDetails(NotFoundException notFoundException) => new()
    {
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        Status = StatusCodes.Status404NotFound,
        Title = notFoundException.Name,
        Detail = notFoundException.Message,
    };
    
    private static ProblemDetails GetProblemDetails(Exception exception) => new()
    {
        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        Status = StatusCodes.Status500InternalServerError,
        Title = exception.GetType().Name,
        Detail = exception.Message,
    };
}