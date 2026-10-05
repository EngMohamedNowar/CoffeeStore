namespace ECommerce.Application.DTOs.Identity;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
