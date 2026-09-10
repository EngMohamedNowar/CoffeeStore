using ECommerce.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        public ActionResult ToActionResult(Result result)
        {
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
              return ToProblem(result.Errors);
            }
        }

        public ActionResult<T> ToActionResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return ToProblem(result.Errors);
            }
        }

        protected static ObjectResult ToProblem(IReadOnlyList<Error> errors)
        {
            var firstError = errors[0];
            var statusCode = firstError.ErrorType switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Forbiden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };
            var problems = new ProblemDetails()
            {
                Detail = firstError.description,
                Title = firstError.code,
                Status = statusCode,
                Extensions = { ["Errors"] = errors }
            };

            return new ObjectResult(problems)
            {
                StatusCode = statusCode
            };

        }

    }
}
