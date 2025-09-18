using Identity.DataLayer.Entities;

namespace Identity.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<int> CreateAsync(User user);
        Task<User> GetAsync(string email, string passwordHash);
    }
}
