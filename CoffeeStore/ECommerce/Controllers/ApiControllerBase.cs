using ECommerce.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiControllerBase : ControllerBase
{
    [NonAction]
    protected ActionResult ToActionResult(Result result)
        => result.IsSuccess ? Ok() : ToProblem(result.Errors);

    [NonAction]
    protected ActionResult<T> ToActionResult<T>(Result<T> result)
        => result.IsSuccess && result.Value is not null ? Ok(result.Value) : ToProblem(result.Errors);

    protected static ObjectResult ToProblem(IReadOnlyList<Error> errors)
    {
        if (errors is null || errors.Count == 0)
        {
            return new ObjectResult(new ProblemDetails
            {
                Detail = "General Failure error has occurred",
                Title = "General.Failure",
                Status = StatusCodes.Status500InternalServerError
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        var firstError = errors[0];

        var statusCode = firstError.ErrorType switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.InvalidCredentials => StatusCodes.Status401Unauthorized,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new ProblemDetails
        {
            Detail = firstError.description,
            Title = firstError.code,
            Status = statusCode
        };

        problem.Extensions["Errors"] = errors;

        return new ObjectResult(problem)
        {
            StatusCode = statusCode
        };
    }
}
