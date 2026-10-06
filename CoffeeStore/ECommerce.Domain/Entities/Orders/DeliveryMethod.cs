using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;

namespace ECommerce.Domain.Entities.Orders;

public sealed class DeliveryMethod : BaseEntity
{
    public const int MaxNameLength = 100;
    public const int MaxDescriptionLength = 500;
    public const int MaxDeliveryTimeLength = 100;

    private DeliveryMethod()
    {
    }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public string EstimatedDeliveryTime { get; private set; } = string.Empty;
    public bool IsAvailable { get; private set; } = true;
    public int DisplayOrder { get; private set; }

    public static Result<DeliveryMethod> Create(
        Guid id,
        string name,
        decimal price,
        string estimatedDeliveryTime,
        string? description = null,
        bool isAvailable = true,
        int displayOrder = 0)
    {
        var invalid = Validate(id, name, price, estimatedDeliveryTime, description);
        if (invalid is not null)
            return Result<DeliveryMethod>.Failure(invalid);

        return Result<DeliveryMethod>.Success(new DeliveryMethod
        {
            Id = id,
            Name = name.Trim(),
            Description = NormalizeDescription(description),
            Price = price,
            EstimatedDeliveryTime = estimatedDeliveryTime.Trim(),
            IsAvailable = isAvailable,
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow
        });
    }

    public Result Update(
        string name,
        decimal price,
        string estimatedDeliveryTime,
        string? description,
        bool isAvailable,
        int displayOrder)
    {
        var invalid = Validate(Id, name, price, estimatedDeliveryTime, description);
        if (invalid is not null)
            return Result.Failure(invalid);

        Name = name.Trim();
        Description = NormalizeDescription(description);
        Price = price;
        EstimatedDeliveryTime = estimatedDeliveryTime.Trim();
        IsAvailable = isAvailable;
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private static Error? Validate(
        Guid id,
        string name,
        decimal price,
        string estimatedDeliveryTime,
        string? description)
    {
        if (id == Guid.Empty)
            return DeliveryMethodErrors.InvalidId;

        if (string.IsNullOrWhiteSpace(name))
            return DeliveryMethodErrors.InvalidName;

        if (name.Trim().Length > MaxNameLength)
            return DeliveryMethodErrors.NameTooLong;

        if (price < 0)
            return DeliveryMethodErrors.InvalidPrice;

        if (string.IsNullOrWhiteSpace(estimatedDeliveryTime))
            return DeliveryMethodErrors.InvalidDeliveryTime;

        if (estimatedDeliveryTime.Trim().Length > MaxDeliveryTimeLength)
            return DeliveryMethodErrors.DeliveryTimeTooLong;

        var trimmedDescription = NormalizeDescription(description);
        if (trimmedDescription is not null && trimmedDescription.Length > MaxDescriptionLength)
            return DeliveryMethodErrors.DescriptionTooLong;

        return null;
    }

    private static string? NormalizeDescription(string? description)
        => string.IsNullOrWhiteSpace(description) ? null : description.Trim();
}
