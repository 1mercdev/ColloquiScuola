using SchoolMeetings.Mobile.Models;

namespace SchoolMeetings.Mobile.Services
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string email, string password);
        Task<AuthResult> RegisterAsync(RegisterRequest request);
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
        Task<string?> GetUserRoleAsync();
        Task<string?> GetUserNameAsync();
        Task<bool> ValidateTokenAsync();
        Task LogoutAsync();
        Task<AuthResult> RegisterAdminAsync(RegisterRequest request);
    }
}