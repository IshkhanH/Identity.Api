using Identity.Core.Models;

namespace Identity.Core.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task SaveRefreshTokenAsync(int userId, string token, DateTime expiresAt, string ip);
        Task<RefreshTokenDto> GetRefreshTokenAsync(string token);
        Task<RefreshTokenDto> GetRevokedTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token);
        Task RevokeAllUserTokensAsync(int userId);
    }
}
