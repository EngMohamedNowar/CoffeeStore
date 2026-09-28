using ECommerce.Application.Common;

namespace ECommerce.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Ok_IsSuccessAndCarriesValue()
    {
        var result = Result<int>.Ok(42);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(42, result.Value);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Fail_IsFailureAndCarriesSingleError()
    {
        var result = Result<string>.Fail(Error.NotFound());

        Assert.True(result.IsFailure);
        Assert.Null(result.Value);
        var error = Assert.Single(result.Errors);
        Assert.Equal(ErrorType.NotFound, error.ErrorType);
    }

    [Fact]
    public void Fail_WithList_KeepsEveryError()
    {
        var errors = new List<Error> { Error.NotFound(), Error.Conflict() };

        var result = Result<int>.Fail(errors);

        Assert.True(result.IsFailure);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public void Ok_WithNoValue_HasEmptyErrorList()
    {
        var result = Result.Ok();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(ErrorType.NotFound, ErrorType.NotFound)]
    [InlineData(ErrorType.Validation, ErrorType.Validation)]
    [InlineData(ErrorType.Unauthorized, ErrorType.Unauthorized)]
    [InlineData(ErrorType.Forbidden, ErrorType.Forbidden)]
    [InlineData(ErrorType.Conflict, ErrorType.Conflict)]
    [InlineData(ErrorType.InvalidCredentials, ErrorType.InvalidCredentials)]
    public void ErrorFactories_PreserveTheErrorType(ErrorType expected, ErrorType factory)
    {
        var error = factory switch
        {
            ErrorType.NotFound => Error.NotFound(),
            ErrorType.Validation => Error.Validation(),
            ErrorType.Unauthorized => Error.Unauthorized(),
            ErrorType.Forbidden => Error.Forbidden(),
            ErrorType.Conflict => Error.Conflict(),
            _ => Error.InvalidCredentials()
        };

        Assert.Equal(expected, error.ErrorType);
    }

    [Fact]
    public void InvalidCredentials_UsesTheExpectedCodeWithoutStrayWhitespace()
    {
        Assert.Equal("General.InvalidCredentials", Error.InvalidCredentials().code);
    }
}
