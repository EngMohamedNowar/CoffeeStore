using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Errors;

namespace ECommerce.Tests.Domain;

public class DeliveryMethodTests
{
    private static Result<DeliveryMethod> Create(
        string name = "Standard",
        decimal price = 25m,
        string estimatedDeliveryTime = "2-4 days",
        string? description = null,
        Guid? id = null)
        => DeliveryMethod.Create(
            id ?? Guid.NewGuid(),
            name,
            price,
            estimatedDeliveryTime,
            description);

    [Fact]
    public void Create_StampesTheBaseEntityDefaults()
    {
        var method = Create().Value!;

        Assert.NotEqual(Guid.Empty, method.Id);
        Assert.Equal(DateTimeKind.Utc, method.CreatedAt.Kind);
        Assert.Null(method.UpdatedAt);
        Assert.False(method.IsDeleted);
    }

    [Fact]
    public void Create_DefaultsToAvailableWithNoDescription()
    {
        var method = Create().Value!;

        Assert.True(method.IsAvailable);
        Assert.Equal("Standard", method.Name);
        Assert.Equal("2-4 days", method.EstimatedDeliveryTime);
        Assert.Null(method.Description);
        Assert.Equal(25m, method.Price);
        Assert.Equal(0, method.DisplayOrder);
    }

    [Fact]
    public void Create_TrimmesTheInputAndStampsCreatedAt()
    {
        var id = Guid.NewGuid();

        var result = Create(name: "  Express  ", estimatedDeliveryTime: " 24 hours ", description: "  Fast  ", id: id);

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal(id, result.Value!.Id);
        Assert.Equal("Express", result.Value.Name);
        Assert.Equal("24 hours", result.Value.EstimatedDeliveryTime);
        Assert.Equal("Fast", result.Value.Description);
        Assert.Equal(DateTimeKind.Utc, result.Value.CreatedAt.Kind);
        Assert.True(result.Value.IsAvailable);
    }

    [Fact]
    public void Create_TurnsAnEmptyDescriptionIntoNull()
    {
        var result = Create(description: "   ");

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.Description);
    }

    [Fact]
    public void Create_RejectsEveryBrokenInvariant()
    {
        Assert.Equal(
            "DeliveryMethod.InvalidId",
            Create(id: Guid.Empty).Errors.Single().code);

        Assert.Equal(
            "DeliveryMethod.InvalidName",
            Create(name: "  ").Errors.Single().code);

        Assert.Equal(
            "DeliveryMethod.NameTooLong",
            Create(name: new string('n', DeliveryMethod.MaxNameLength + 1)).Errors.Single().code);

        Assert.Equal(
            "DeliveryMethod.InvalidPrice",
            Create(price: -1).Errors.Single().code);

        Assert.Equal(
            "DeliveryMethod.InvalidDeliveryTime",
            Create(estimatedDeliveryTime: "").Errors.Single().code);

        Assert.Equal(
            "DeliveryMethod.DeliveryTimeTooLong",
            Create(estimatedDeliveryTime: new string('t', DeliveryMethod.MaxDeliveryTimeLength + 1)).Errors.Single().code);

        Assert.Equal(
            "DeliveryMethod.DescriptionTooLong",
            Create(description: new string('d', DeliveryMethod.MaxDescriptionLength + 1)).Errors.Single().code);
    }

    [Fact]
    public void Update_AppliesTheChangesAndStampsUpdatedAt()
    {
        var method = Create().Value!;

        var result = method.Update("Same Day", 40m, "next day", "Courier", false, 3);

        Assert.True(result.IsSuccess, string.Join(", ", result.Errors.Select(e => e.code)));
        Assert.Equal("Same Day", method.Name);
        Assert.Equal(40m, method.Price);
        Assert.Equal("next day", method.EstimatedDeliveryTime);
        Assert.Equal("Courier", method.Description);
        Assert.False(method.IsAvailable);
        Assert.Equal(3, method.DisplayOrder);
        Assert.NotNull(method.UpdatedAt);
    }

    [Fact]
    public void Update_KeepsThePreviousValues_WhenTheInputIsInvalid()
    {
        var method = Create().Value!;
        var nameBefore = method.Name;

        var result = method.Update("  ", -5, "next day", null, true, 0);

        Assert.True(result.IsFailure);
        Assert.Equal(nameBefore, method.Name);
        Assert.Equal(25m, method.Price);
    }

    [Fact]
    public void DeliveryMethodErrors_AreAllValidationErrorsAndShareOnePrefix()
    {
        var errors = new Error[]
        {
            DeliveryMethodErrors.InvalidId,
            DeliveryMethodErrors.InvalidName,
            DeliveryMethodErrors.NameTooLong,
            DeliveryMethodErrors.InvalidPrice,
            DeliveryMethodErrors.InvalidDeliveryTime,
            DeliveryMethodErrors.DeliveryTimeTooLong,
            DeliveryMethodErrors.DescriptionTooLong
        };

        Assert.All(errors, error =>
        {
            Assert.Equal(ErrorType.Validation, error.ErrorType);
            Assert.StartsWith("DeliveryMethod.", error.code);
            Assert.False(string.IsNullOrWhiteSpace(error.description));
        });

        Assert.Equal(errors.Length, errors.Select(e => e.code).Distinct().Count());
    }

    [Fact]
    public void DeliveryMethodErrors_IncludeNotFoundAndConflict()
    {
        Assert.Equal(ErrorType.NotFound, DeliveryMethodErrors.NotFound.ErrorType);
        Assert.Equal(ErrorType.Conflict, DeliveryMethodErrors.NameAlreadyExists.ErrorType);
    }
}
