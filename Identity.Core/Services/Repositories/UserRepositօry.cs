using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Core.Interfaces;
using Identity.DataLayer.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Identity.Core.Services.Repositories
{
    internal class UserRepositօry : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepositօry(IConfiguration configuration)
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

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("sp_GetUserById", conn) { CommandType = CommandType.StoredProcedure })
                {
                    AddParam(cmd, "@UserId", id);

                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync() ? MapToUser(reader) : null;
                    }
                }
            }
        }

        public async Task<IEnumerable<User>> GetListAsync()
        {
            var users = new List<User>();
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("sp_GetAllUsers", conn) { CommandType = CommandType.StoredProcedure })
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(MapToUser(reader));
                        }

                        return users;
                    }
                }
            }
        }

        public async Task UpdateAsync(User user)
        {
            using var conn = new SqlConnection(_connectionString);
            {
                using (var cmd = new SqlCommand("sp_UpdateUser", conn) { CommandType = CommandType.StoredProcedure })
                {
                    AddParam(cmd, "@Email", user.Email);
                    AddParam(cmd, "@Password", user.Password);
                   

                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    await cmd.ExecuteNonQueryAsync();
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
                Id = (int)reader["Id"],
                
                Email = reader["Email"].ToString(),
                Password = reader["Password"].ToString(),
                CreatedDate = (DateTime)reader["CreatedDate"],
                ModifiedDate = (DateTime)reader["ModifiedDate"],
                IsDeleted = (bool)reader["IsDeleted"]
            };
        }
    }
}
