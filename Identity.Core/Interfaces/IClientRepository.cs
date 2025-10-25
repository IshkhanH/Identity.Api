using Identity.Core.Enums;
using Identity.Core.Models.Client;

namespace Identity.Core.Interfaces
{
    public interface IClientRepository
    {
        Task<int> CreateAsync(ClientCreate client);
        Task<ClientDetails> GetAsync(int id);
        Task<IEnumerable<ClientDetails>> GetListAsync();
        Task<UpdateStatus> UpdateAsync(ClientUpdate client);
        Task<DeleteStatus> DeleteAsync(int id, int userId);
        Task<IEnumerable<ClientDetails>> GetByUserIdAsync(int userId);

    }
}
