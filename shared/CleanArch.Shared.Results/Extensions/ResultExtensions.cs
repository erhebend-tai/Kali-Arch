using Microsoft.AspNetCore.Mvc;

namespace CleanArch.Shared.Results.Extensions;

public static class ResultExtensions
{
    /// <summary>Aggregate multiple results — fails with the first error.</summary>
    public static Result Combine(params Result[] results)
    {
        var failures = results.Where(r => r.IsFailure).ToList();
        return failures.Count == 0
            ? Result.Success()
            : Result.Failure(failures[0].Error);
    }

    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        Func<T, IActionResult> onSuccess) =>
        result.IsSuccess
            ? onSuccess(result.Value)
            : result.Error.ToActionResult();

    public static IActionResult ToActionResult(this Error error) =>
        error.Type switch
        {
            ErrorType.NotFound =>
                new NotFoundObjectResult(error.ToProblemDetails()),
            ErrorType.Validation =>
                new BadRequestObjectResult(error.ToProblemDetails()),
            ErrorType.Conflict =>
                new ConflictObjectResult(error.ToProblemDetails()),
            ErrorType.Unauthorized =>
                new UnauthorizedObjectResult(error.ToProblemDetails()),
            _ => new ObjectResult(error.ToProblemDetails())
                 { StatusCode = 500 }
        };

    private static ProblemDetails ToProblemDetails(this Error error) =>
        new()
        {
            Title = error.Type.ToString(),
            Detail = error.Description,
            Extensions = { ["errorCode"] = error.Code }
        };
}
