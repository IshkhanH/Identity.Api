using Identity.Core.Enums;
using Identity.Core.Models.User;

namespace Identity.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<int> CreateAsync(RegistrationRequest user);
        Task<UserDetails> GetAsync(string email, string passwordHash);
        Task<UserDetails> GetAsync(int userId);
        Task<DeleteStatus> DeleteAsync(int id, int userId);
        Task<IEnumerable<UserDetails>> GetListAsync();
    }
}
