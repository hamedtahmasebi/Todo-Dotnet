namespace TodoApi.Models.Auth;

public class RegisterDto
{
    public required string Email { get; set; } = string.Empty;
    public required string Password { get; set; } = string.Empty;
}


public class LoginDto
{
    public required string Email { get; set; } = string.Empty;
    public required string Password { get; set; } = string.Empty;
}


public class AuthResponseDto
{
    public required string Token { get; set; } = string.Empty;
    public required DateTime Expiration { get; set; }
    public required string UserId { get; set; } = string.Empty;
    public required string Email { get; set; } = string.Empty;
}


public class ChangePasswordDto
{
    public required string CurrentPassword { get; set; } = string.Empty;
    public required string NewPassword { get; set; } = string.Empty;
}

