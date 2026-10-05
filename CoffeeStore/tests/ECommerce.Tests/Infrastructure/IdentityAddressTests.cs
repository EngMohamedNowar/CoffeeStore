using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Identity;

namespace ECommerce.Tests.Infrastructure;

public class IdentityAddressTests
{
    [Fact]
    public async Task AddAddressAsync_TheFirstAddressBecomesTheDefaultOne()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();

        var result = await stack.Identity.AddAddressAsync(IdentityStack.DefaultEmail, NewAddress());

        Assert.True(result.IsSuccess, Describe(result.Errors));
        Assert.True(result.Value!.IsDefault);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal("Home", result.Value.Label);
        Assert.Equal("Sara Nasser", result.Value.FullName);
    }

    [Fact]
    public async Task AddAddressAsync_PersistsEveryField()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();

        var result = await stack.Identity.AddAddressAsync(IdentityStack.DefaultEmail, NewAddress("Nile St"));

        var stored = Assert.Single(await ListAsync(stack));
        Assert.Equal(result.Value!.Id, stored.Id);
        Assert.Equal("Egypt", stored.Country);
        Assert.Equal("Cairo", stored.City);
        Assert.Equal("Nile St", stored.Street);
        Assert.Equal("12", stored.Building);
        Assert.Equal("Ring twice", stored.Notes);
    }

    [Fact]
    public async Task AddAddressAsync_ASecondAddressDoesNotStealTheDefault()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();

        await stack.Identity.AddAddressAsync(IdentityStack.DefaultEmail, NewAddress("First St"));
        await stack.Identity.AddAddressAsync(IdentityStack.DefaultEmail, NewAddress("Second St"));

        var defaultAddress = await stack.Identity.GetCurrentUserAddressAsync(IdentityStack.DefaultEmail);

        Assert.Equal("First St", defaultAddress.Value!.Street);
        Assert.Equal(2, (await ListAsync(stack)).Count);
    }

    [Fact]
    public async Task AddAddressAsync_ExplicitDefaultMovesItOffThePreviousAddress()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();

        await stack.Identity.AddAddressAsync(IdentityStack.DefaultEmail, NewAddress("First St"));
        await stack.Identity.AddAddressAsync(IdentityStack.DefaultEmail, NewAddress("Second St", isDefault: true));

        var defaultAddress = await stack.Identity.GetCurrentUserAddressAsync(IdentityStack.DefaultEmail);
        var all = await ListAsync(stack);

        Assert.Equal("Second St", defaultAddress.Value!.Street);
        Assert.Single(all.Where(a => a.IsDefault));
        Assert.Equal("Second St", all[0].Street);
    }

    [Fact]
    public async Task AddAddressAsync_RequiresACityAndAStreet()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();

        var withoutStreet = NewAddress(street: "   ");
        var withoutCity = NewAddress();
        withoutCity.City = string.Empty;

        var streetResult = await stack.Identity.AddAddressAsync(IdentityStack.DefaultEmail, withoutStreet);
        var cityResult = await stack.Identity.AddAddressAsync(IdentityStack.DefaultEmail, withoutCity);

        Assert.True(streetResult.IsFailure);
        Assert.Equal(ErrorType.Validation, Assert.Single(streetResult.Errors).ErrorType);
        Assert.True(cityResult.IsFailure);
        Assert.Empty(await ListAsync(stack));
    }

    [Fact]
    public async Task AddAddressAsync_FailsForAnUnknownUser()
    {
        using var stack = new IdentityStack();

        var result = await stack.Identity.AddAddressAsync("nobody@store.com", NewAddress());

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, Assert.Single(result.Errors).ErrorType);
    }

    [Fact]
    public async Task GetDefaultAsync_FailsWhenTheUserHasNoAddresses()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();

        var result = await stack.Identity.GetCurrentUserAddressAsync(IdentityStack.DefaultEmail);

        Assert.True(result.IsFailure);
        Assert.Equal("Address.NotFound", Assert.Single(result.Errors).code);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetAddressesAsync_FailsForAnUnknownUser()
    {
        using var stack = new IdentityStack();

        var result = await stack.Identity.GetAddressesAsync("nobody@store.com");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, Assert.Single(result.Errors).ErrorType);
    }

    [Fact]
    public async Task UpdateAddressAsync_ChangesTheStoredValues()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();

        var added = await stack.Identity.AddAddressAsync(IdentityStack.DefaultEmail, NewAddress("Old St"));
        var update = NewAddress("New St");
        update.Label = "Office";
        update.City = "Giza";

        var result = await stack.Identity.UpdateAddressAsync(IdentityStack.DefaultEmail, added.Value!.Id, update);

        Assert.True(result.IsSuccess, Describe(result.Errors));
        var stored = Assert.Single(await ListAsync(stack));
        Assert.Equal("New St", stored.Street);
        Assert.Equal("Office", stored.Label);
        Assert.Equal("Giza", stored.City);
    }

    [Fact]
    public async Task UpdateAddressAsync_MovesTheDefaultToTheUpdatedAddress()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();

        var first = await stack.Identity.AddAddressAsync(IdentityStack.DefaultEmail, NewAddress("First St"));
        var second = await stack.Identity.AddAddressAsync(IdentityStack.DefaultEmail, NewAddress("Second St"));

        var makeSecondDefault = NewAddress("Second St");
        makeSecondDefault.IsDefault = true;
        await stack.Identity.UpdateAddressAsync(IdentityStack.DefaultEmail, second.Value!.Id, makeSecondDefault);

        var all = await ListAsync(stack);
        var defaultAddress = await stack.Identity.GetCurrentUserAddressAsync(IdentityStack.DefaultEmail);

        Assert.Equal(second.Value.Id, defaultAddress.Value!.Id);
        Assert.DoesNotContain(all, a => a.Id == first.Value!.Id && a.IsDefault);
        Assert.Single(all.Where(a => a.IsDefault));
    }

    [Fact]
    public async Task UpdateAddressAsync_CannotTouchAnotherUsersAddress()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();
        await stack.CreateUserAsync("neighbour@store.com");

        var theirs = await stack.Identity.AddAddressAsync("neighbour@store.com", NewAddress("Theirs St"));

        var result = await stack.Identity.UpdateAddressAsync(
            IdentityStack.DefaultEmail,
            theirs.Value!.Id,
            NewAddress("Hacked St"));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, Assert.Single(result.Errors).ErrorType);

        var theirStored = await stack.Identity.GetCurrentUserAddressAsync("neighbour@store.com");
        Assert.Equal("Theirs St", theirStored.Value!.Street);
    }

    [Fact]
    public async Task DeleteAddressAsync_RemovesTheAddress()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();

        var added = await stack.Identity.AddAddressAsync(IdentityStack.DefaultEmail, NewAddress());

        var result = await stack.Identity.DeleteAddressAsync(IdentityStack.DefaultEmail, added.Value!.Id);

        Assert.True(result.IsSuccess, Describe(result.Errors));
        Assert.Empty(await ListAsync(stack));
        Assert.True((await stack.Identity.GetCurrentUserAddressAsync(IdentityStack.DefaultEmail)).IsFailure);
    }

    [Fact]
    public async Task DeleteAddressAsync_CannotRemoveAnotherUsersAddress()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();
        await stack.CreateUserAsync("neighbour@store.com");

        var theirs = await stack.Identity.AddAddressAsync("neighbour@store.com", NewAddress("Theirs St"));

        var result = await stack.Identity.DeleteAddressAsync(IdentityStack.DefaultEmail, theirs.Value!.Id);

        Assert.True(result.IsFailure);
        var stillThere = await stack.Identity.GetCurrentUserAddressAsync("neighbour@store.com");
        Assert.Equal("Theirs St", stillThere.Value!.Street);
    }

    [Fact]
    public async Task DeleteAddressAsync_FailsForAnUnknownAddressId()
    {
        using var stack = new IdentityStack();
        await stack.CreateUserAsync();

        var result = await stack.Identity.DeleteAddressAsync(IdentityStack.DefaultEmail, Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, Assert.Single(result.Errors).ErrorType);
    }

    private static async Task<IReadOnlyList<AddressDto>> ListAsync(IdentityStack stack)
    {
        var result = await stack.Identity.GetAddressesAsync(IdentityStack.DefaultEmail);
        Assert.True(result.IsSuccess, Describe(result.Errors));
        return result.Value!;
    }

    private static AddressDto NewAddress(string street = "Main St", bool isDefault = false) => new()
    {
        Label = "Home",
        FirstName = "Sara",
        LastName = "Nasser",
        Country = "Egypt",
        City = "Cairo",
        Street = street,
        Building = "12",
        Notes = "Ring twice",
        IsDefault = isDefault
    };

    private static string Describe(IReadOnlyList<Error> errors)
        => string.Join(", ", errors.Select(e => $"{e.code}: {e.description}"));
}
