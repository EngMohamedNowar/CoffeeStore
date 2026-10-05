using ECommerce.Application.DTOs.Identity;

namespace ECommerce.Tests.Application;

public class AddressDtoTests
{
    [Fact]
    public void FullName_JoinsTheFirstAndLastName()
    {
        var address = new AddressDto { FirstName = "Sara", LastName = "Nasser" };

        Assert.Equal("Sara Nasser", address.FullName);
    }

    [Fact]
    public void FullName_FallsBackToTheFirstNameWhenThereIsNoLastName()
    {
        var withEmptyLastName = new AddressDto { FirstName = "Sara", LastName = string.Empty };
        var withWhitespaceLastName = new AddressDto { FirstName = "Sara", LastName = "   " };

        Assert.Equal("Sara", withEmptyLastName.FullName);
        Assert.Equal("Sara", withWhitespaceLastName.FullName);
    }

    [Fact]
    public void Defaults_AreEmptyAndNotDefault()
    {
        var address = new AddressDto();

        Assert.Equal(Guid.Empty, address.Id);
        Assert.Equal(string.Empty, address.Label);
        Assert.False(address.IsDefault);
        Assert.Equal(string.Empty, address.FullName);
    }
}
