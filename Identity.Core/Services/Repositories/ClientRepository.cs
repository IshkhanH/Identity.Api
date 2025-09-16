using Identity.Core.Interfaces;
using Identity.DataLayer.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Identity.Core.Services.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateAsync(Client client)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("sp_AddClient", conn) { CommandType = CommandType.StoredProcedure })
                {
                    AddParam(cmd, "@FirstName", client.FirstName);
                    AddParam(cmd, "@LastName", client.LastName);
                    AddParam(cmd, "@SurName", client.SurName);
                    AddParam(cmd, "@Passport", client.Passport);
                    AddParam(cmd, "@PhoneNumber", client.PhoneNumber);
                    AddParam(cmd, "@Address", client.Address);
                    AddParam(cmd, "@Mail", client.Mail);
                    AddParam(cmd, "@UserId", client.UserId); //client.UserId);

                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<Client> GetAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("sp_GetClientById", conn) { CommandType = CommandType.StoredProcedure })
                {
                    AddParam(cmd, "@Id", id);

                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync() ? MapToClient(reader) : null;
                    }
                }
            }
        }

        public async Task<IEnumerable<Client>> GetListAsync()
        {
            var clients = new List<Client>();
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("sp_GetAllClients", conn) { CommandType = CommandType.StoredProcedure })
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            clients.Add(MapToClient(reader));
                        }

                        return clients;
                    }
                }
            }
        }

        public async Task UpdateAsync(Client client)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("sp_UpdateClient", conn) { CommandType = CommandType.StoredProcedure })
                {
                    AddParam(cmd, "@Id", client.Id);
                    AddParam(cmd, "@FirstName", client.FirstName);
                    AddParam(cmd, "@LastName", client.LastName);
                    AddParam(cmd, "@SurName", client.SurName);
                    AddParam(cmd, "@Passport", client.Passport);
                    AddParam(cmd, "@PhoneNumber", client.PhoneNumber);
                    AddParam(cmd, "@Address", client.Address);
                    AddParam(cmd, "@Mail", client.Mail);
                    AddParam(cmd, "@UserId", client.UserId); //client.UserId);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = new SqlCommand("sp_DeleteClient", conn) { CommandType = CommandType.StoredProcedure })
                {
                    AddParam(cmd, "@Id", id);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private static void AddParam(SqlCommand cmd, string name, object value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        private static Client MapToClient(SqlDataReader reader)
        {
            return new Client
            {
                Id = (int)reader["Id"],
                FirstName = reader["FirstName"].ToString(),
                LastName = reader["LastName"].ToString(),
                SurName = reader["SurName"].ToString(),
                Passport = reader["Passport"].ToString(),
                PhoneNumber = reader["PhoneNumber"].ToString(),
                Address = reader["Address"].ToString(),
                Mail = reader["Mail"].ToString(),
                UserId = (int)reader["UserId"],
                CreatedDate = (DateTime)reader["CreatedDate"],
                ModifiedDate = (DateTime)reader["ModifiedDate"],
                IsDeleted = (bool)reader["IsDeleted"]
            };
        }
    }
}
