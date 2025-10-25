using System.Security.Claims;
using Identity.Core.Enums;
using Identity.Core.Models.Client;

namespace Identity.Core.Interfaces
{
    public interface IClientService
    {
        Task<int> CreateAsync(ClientCreate model);
        Task<UpdateStatus> UpdateAsync(int id, ClientUpdate model, ClaimsPrincipal user);
        Task<DeleteStatus> DeleteAsync(int id, int userId);
        Task<ClientDetails> GetAsync(int id);
        Task<IEnumerable<ClientDetails>> GetListAsync();
        Task<IEnumerable<ClientDetails>> GetByUserIdAsync(int userId);
        Task<byte[]> ExportExcelClientsAsync();

    }
}
