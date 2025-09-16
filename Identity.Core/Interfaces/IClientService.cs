using Identity.Core.Models.Client;

namespace Identity.Core.Interfaces
{
    public interface IClientService
    {
        Task<int> CreateAsync(ClientCreate model);
        Task UpdateAsync(int id, ClientUpdate model);
        Task DeleteAsync(int id);
        Task<ClientDetails> GetAsync(int id);
        Task<IEnumerable<ClientDetails>> GetListAsync();
    }
}
