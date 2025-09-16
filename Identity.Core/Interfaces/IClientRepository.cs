using Identity.DataLayer.Entities;

namespace Identity.Core.Interfaces
{
    public interface IClientRepository
    {
        Task<int> CreateAsync(Client client);
        Task<Client> GetAsync(int id);
        Task<IEnumerable<Client>> GetListAsync();
        Task UpdateAsync(Client client);
        Task DeleteAsync(int id);
    }
}
