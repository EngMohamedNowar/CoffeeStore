using ECommerce.Api.Controllers;
using ECommerce.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Tests.Api;

public class ApiControllerBaseTests
{
    private sealed class TestController : ApiControllerBase
    {
        public ActionResult Plain(Result result) => ToActionResult(result);

        public ActionResult<T> Typed<T>(Result<T> result) => ToActionResult(result);
    }

    [Fact]
    public void ToActionResult_ReturnsOk_WhenTheResultSucceeds()
    {
        var action = new TestController().Plain(Result.Ok());

        Assert.IsType<OkResult>(action);
    }

    [Fact]
    public void ToActionResult_ReturnsOkWithThePayload_WhenTheResultSucceeds()
    {
        var action = new TestController().Typed(Result<string>.Ok("payload"));

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        Assert.Equal("payload", ok.Value);
    }

    [Fact]
    public void ToActionResult_FallsBackToA500Problem_WhenThereAreNoErrors()
    {
        var action = new TestController().Typed(Result<string>.Fail(Array.Empty<Error>()));

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(500, problem.StatusCode);
        var details = Assert.IsType<ProblemDetails>(problem.Value);
        Assert.Equal("General.Failure", details.Title);
    }

    [Theory]
    [InlineData(ErrorType.NotFound, 404)]
    [InlineData(ErrorType.Validation, 400)]
    [InlineData(ErrorType.Unauthorized, 401)]
    [InlineData(ErrorType.Forbidden, 403)]
    [InlineData(ErrorType.InvalidCredentials, 401)]
    [InlineData(ErrorType.Conflict, 409)]
    [InlineData(ErrorType.Failure, 500)]
    public void ToProblem_MapsEveryErrorTypeToItsHttpStatus(ErrorType errorType, int expectedStatus)
    {
        var error = new Error("Shop.Code", "Something went wrong.", errorType);

        var action = new TestController().Typed(Result<string>.Fail(error));

        var problem = Assert.IsType<ObjectResult>(action.Result);
        Assert.Equal(expectedStatus, problem.StatusCode);

        var details = Assert.IsType<ProblemDetails>(problem.Value);
        Assert.Equal(expectedStatus, details.Status);
        Assert.Equal("Shop.Code", details.Title);
        Assert.Equal("Something went wrong.", details.Detail);
        Assert.Equal(new Error[] { error }, details.Extensions["Errors"]);
    }
}
