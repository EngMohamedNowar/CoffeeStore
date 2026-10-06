namespace ECommerce.Domain.Shared;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<Error> Errors { get; }

    protected Result(bool isSuccess, IReadOnlyList<Error> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static Result Ok()
        => new(true, Array.Empty<Error>());

    public static Result Fail(IReadOnlyList<Error> errors)
        => new(false, errors);

    public static Result Fail(Error error)
        => new(false, new[] { error });

    public static Result Success()
        => Ok();

    public static Result Failure(Error error)
        => Fail(error);

    public static Result Failure(IReadOnlyList<Error> errors)
        => Fail(errors);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(
        T? value,
        bool isSuccess,
        IReadOnlyList<Error> errors)
        : base(isSuccess, errors)
    {
        Value = value;
    }

    public static Result<T> Ok(T value)
        => new(value, true, Array.Empty<Error>());

    public static Result<T> Success(T value)
        => Ok(value);

    public static Result<IReadOnlyList<T>> Ok(IReadOnlyList<T> value)
        => new(value, true, Array.Empty<Error>());

    public new static Result<T> Fail(IReadOnlyList<Error> errors)
        => new(default, false, errors);

    public new static Result<T> Fail(Error error)
        => new(default, false, new[] { error });

    public new static Result<T> Failure(Error error)
        => Fail(error);

    public new static Result<T> Failure(IReadOnlyList<Error> errors)
        => Fail(errors);
}
