using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.DataLayer.Entities;

namespace Identity.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<int> CreateAsync(User user);
        Task<User> GetAsync(int id);
        Task<IEnumerable<User>> GetListAsync();
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
    }
}
