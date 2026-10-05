using ECommerce.Api.Controllers;
using ECommerce.Application.DTOs.Identity;
using ECommerce.Tests.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Tests.Api;

public class AddressesControllerTests
{
    private static AddressDto NewAddress(string street = "Main St", bool isDefault = false) => new()
    {
        Label = "Home",
        FirstName = "Sara",
        LastName = "Nasser",
        Country = "Egypt",
        City = "Cairo",
        Street = street,
        IsDefault = isDefault
    };

    private static async Task<(AddressesController controller, IdentityStack stack)> BuildAsync()
    {
        var stack = new IdentityStack();
        await stack.CreateUserAsync();

        var controller = new AddressesController(stack.Identity)
        {
            ControllerContext = ControllerContextFactory.WithEmail(IdentityStack.DefaultEmail)
        };

        return (controller, stack);
    }

    private static T Value<T>(ActionResult<T> action)
        => Assert.IsType<T>(Assert.IsType<OkObjectResult>(action.Result).Value);

    private static ProblemDetails Problem(ActionResult action)
        => Assert.IsType<ProblemDetails>(Assert.IsType<ObjectResult>(action).Value);

    [Fact]
    public async Task GetAll_ReturnsAnEmptyList_WhenTheUserHasNoAddress()
    {
        var (controller, stack) = await BuildAsync();
        using var _ = stack;

        var action = await controller.GetAll();

        var addresses = Assert.IsAssignableFrom<IReadOnlyList<AddressDto>>(
            Assert.IsType<OkObjectResult>(action.Result).Value);
        Assert.Empty(addresses);
    }

    [Fact]
    public async Task GetDefault_ReturnsNotFound_WhenTheUserHasNoAddress()
    {
        var (controller, stack) = await BuildAsync();
        using var _ = stack;

        var action = await controller.GetDefault();

        Assert.Equal(404, Assert.IsType<ObjectResult>(action.Result).StatusCode);
        Assert.Equal("Address.NotFound", Problem(action.Result!).Title);
    }

    [Fact]
    public async Task Create_StoresTheAddressAndMakesTheFirstOneTheDefault()
    {
        var (controller, stack) = await BuildAsync();
        using var _ = stack;

        var created = Value(await controller.Create(NewAddress()));

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.True(created.IsDefault);

        var all = await controller.GetAll();
        var addresses = Assert.IsAssignableFrom<IReadOnlyList<AddressDto>>(
            Assert.IsType<OkObjectResult>(all.Result).Value);
        Assert.Single(addresses);

        var defaultAction = await controller.GetDefault();
        Assert.Equal(created.Id, Value(defaultAction).Id);
    }

    [Fact]
    public async Task Create_MovesTheDefaultFlagToTheNewestDefaultAddress()
    {
        var (controller, stack) = await BuildAsync();
        using var _ = stack;

        var first = Value(await controller.Create(NewAddress()));
        var second = Value(await controller.Create(NewAddress("Second St", isDefault: true)));

        Assert.True(second.IsDefault);

        var refreshedFirst = await controller.GetDefault();
        Assert.Equal(second.Id, Value(refreshedFirst).Id);
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public async Task Update_ChangesTheStoredAddress()
    {
        var (controller, stack) = await BuildAsync();
        using var _ = stack;

        var created = Value(await controller.Create(NewAddress()));
        var updated = Value(await controller.Update(created.Id, NewAddress("Other St")));

        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("Other St", updated.Street);

        var reloaded = await controller.GetDefault();
        Assert.Equal("Other St", Value(reloaded).Street);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_ForAnAddressTheUserDoesNotOwn()
    {
        var (controller, stack) = await BuildAsync();
        using var _ = stack;

        await controller.Create(NewAddress());

        var action = await controller.Update(Guid.NewGuid(), NewAddress("Other St"));

        Assert.Equal(404, Assert.IsType<ObjectResult>(action.Result).StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesTheAddress()
    {
        var (controller, stack) = await BuildAsync();
        using var _ = stack;

        var created = Value(await controller.Create(NewAddress()));

        var action = await controller.Delete(created.Id);

        Assert.IsType<OkResult>(action);

        var all = await controller.GetAll();
        var addresses = Assert.IsAssignableFrom<IReadOnlyList<AddressDto>>(
            Assert.IsType<OkObjectResult>(all.Result).Value);
        Assert.Empty(addresses);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_ForAnAddressThatIsAlreadyGone()
    {
        var (controller, stack) = await BuildAsync();
        using var _ = stack;

        var action = await controller.Delete(Guid.NewGuid());

        Assert.Equal(404, Assert.IsType<ObjectResult>(action).StatusCode);
    }

    [Fact]
    public async Task EveryAction_RequiresAnEmailClaim()
    {
        var stack = new IdentityStack();
        using var _ = stack;

        var controller = new AddressesController(stack.Identity)
        {
            ControllerContext = ControllerContextFactory.WithoutEmail()
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => controller.GetAll());
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => controller.GetDefault());
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => controller.Create(NewAddress()));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => controller.Update(Guid.NewGuid(), NewAddress()));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => controller.Delete(Guid.NewGuid()));
    }
}
