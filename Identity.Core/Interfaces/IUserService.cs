using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Core.Models.Client;
using Identity.Core.Models.User;

namespace Identity.Core.Interfaces
{
    public interface IUserService
    {
        Task<int> CreateAsync(UserCreate model);
        Task UpdateAsync(int id, UserUpdate model);
        Task DeleteAsync(int id);
        Task<UserDetails> GetAsync(int id);
        Task<IEnumerable<UserDetails>> GetListAsync();
        
    }
}
