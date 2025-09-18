using Identity.Core.Models.User;

namespace Identity.Core.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegistrationAsync(RegistrationRequest request);
    }
}
