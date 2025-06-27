using TodoApi.Models.User;

namespace TodoApi.Services.Auth;



public interface ITokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
}
