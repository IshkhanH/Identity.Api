using System.Data;
using Identity.Core.Interfaces;
using Identity.DataLayer.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Identity.Core.Services.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateAsync(User user)
        {
            using var conn = new SqlConnection(_connectionString);
            {
                using (var cmd = new SqlCommand("AddUser", conn) { CommandType = CommandType.StoredProcedure})
                {
                    AddParam(cmd, "@Email", user.Email);
                    AddParam(cmd, "@Password", user.Password);

                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<User> GetAsync(string email, string passwordHash)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("sp_GetUserByEmailAndPasswordHash", conn) { CommandType = CommandType.StoredProcedure })
                {
                    AddParam(cmd, "@Email", email);
                    AddParam(cmd, "@Password", passwordHash);

                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync() ? MapToUser(reader) : null;
                    }
                }
            }
        }

        private static void AddParam(SqlCommand cmd, string name, object value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        private static User MapToUser(SqlDataReader reader)
        {
            return new User
            {
                Id = (int)reader["Id"]
            };
        }
    }
}
