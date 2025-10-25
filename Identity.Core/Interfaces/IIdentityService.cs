using Identity.Core.Enums;
using Identity.Core.Models.User;

namespace Identity.Core.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request, string ipAddress);
        Task<AuthResponse> RegistrationAsync(RegistrationRequest request, string ipAddress);
        Task<DeleteStatus> DeleteAsync(int id, int userId);
        Task<IEnumerable<UserDetails>> GetListAsync();
        Task<AuthResponse> RefreshTokenAsync(string refreshToken, string ipAddress);
        Task RevokeTokenAsync(string refreshToken);
        Task RevokeAllUserTokensAsync(int userId);
    }
}
