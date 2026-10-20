using System.Net;
using System.Text.Json;
using CleanArch.Shared.Results;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CleanArch.Shared.Infrastructure.Middleware;

/// <summary>
/// Single exception→ProblemDetails translation point.
/// Error codes from the Shared.Results model are carried on
/// the ProblemDetails extensions so every entry point
/// (API, Functions, workers) responds consistently.
/// </summary>
public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var error = Classify(ex);

            var status = error.Type switch
            {
                ErrorType.Validation => HttpStatusCode.BadRequest,
                ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
                ErrorType.Forbidden => HttpStatusCode.Forbidden,
                ErrorType.NotFound => HttpStatusCode.NotFound,
                ErrorType.Conflict => HttpStatusCode.Conflict,
                ErrorType.Domain => HttpStatusCode.UnprocessableEntity,
                _ => HttpStatusCode.InternalServerError
            };

            if (status == HttpStatusCode.InternalServerError)
                logger.LogError(ex, "Unhandled exception");
            else
                logger.LogWarning(ex,
                    "Request failed [{Code}]: {Message}",
                    error.Code, ex.Message);

            var problem = new ProblemDetails
            {
                Status = (int)status,
                Title = error.Type.ToString(),
                Detail = status == HttpStatusCode.InternalServerError
                    ? "An unexpected error occurred."
                    : ex.Message,
                Instance = context.Request.Path
            };

            problem.Extensions["code"] = error.Code;
            problem.Extensions["traceId"] = context.TraceIdentifier;

            if (ex is ValidationException vex)
            {
                problem.Extensions["errors"] = vex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());
            }

            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(problem));
        }
    }

    private static Error Classify(Exception ex)
    {
        if (ex is ValidationException vex)
            return Error.Validation(
                "Validation.Failed",
                string.Join("; ", vex.Errors
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")));

        return ex switch
        {
            _ when ex.GetType().Name == "NotFoundException"
                => Error.NotFound(
                    "Resource.NotFound", ex.Message),
            _ when ex.GetType().Name == "ConflictException"
                => Error.Conflict(
                    "Resource.Conflict", ex.Message),
            _ when ex.GetType().Name ==
                    "ApplicationUnauthorizedException"
                || ex is UnauthorizedAccessException
                => Error.Unauthorized(
                    "Auth.Unauthorized", ex.Message),
            _ when IsDomainException(ex)
                => Error.Domain(
                    "Domain.RuleViolation", ex.Message),
            _ => Error.Failure(
                "Server.InternalError", ex.Message)
        };
    }

    private static bool IsDomainException(Exception ex) =>
        ex.GetType().Name.EndsWith("DomainException")
        || ex.GetType().BaseType?.Name.EndsWith("DomainException")
            == true;
}
