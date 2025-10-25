using System.Data;
using Identity.Core.Enums;
using Identity.Core.Interfaces;
using Identity.Core.Models.User;
using Identity.DataLayer.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace Identity.DataLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        private static void AddParam(SqlCommand cmd, string name, object value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        private static UserDetails MapToUserDetails(SqlDataReader reader)
        {
            return new UserDetails
            {
                Id = Convert.ToInt32(reader["Id"]),
                Email = reader["Email"]?.ToString() ?? string.Empty,
                FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                LastName = reader["LastName"]?.ToString() ?? string.Empty,
                SurName = reader["SurName"]?.ToString(),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }

 //CREATE ASYNC
        public async Task<int> CreateAsync(RegistrationRequest user)
        {
            using var conn = new SqlConnection(_connectionString);
            {
                await conn.OpenAsync();

                var checkEmailCmd = new SqlCommand("SELECT COUNT(1) FROM Users WHERE Email = @Email", conn);
                checkEmailCmd.Parameters.AddWithValue("@Email", user.Email);

                var count = (int)await checkEmailCmd.ExecuteScalarAsync();

                if (count > 0)
                    throw new Exception("Նշված Email-ը արդեն գրանցված է:");

                using var cmd = new SqlCommand("sp_RegisterUser", conn){ CommandType = CommandType.StoredProcedure };
                AddParam(cmd, "@FirstName", user.FirstName);
                AddParam(cmd, "@LastName", user.LastName);
                AddParam(cmd, "@SurName", user.SurName);
                AddParam(cmd, "@Email", user.Email);
                AddParam(cmd, "@Password", user.Password);

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }
//GET ASYNC
        public async Task<UserDetails> GetAsync(string email, string passwordHash)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetUserByEmailAndPasswordHash", conn){ CommandType = CommandType.StoredProcedure };
            AddParam(cmd, "@Email", email);
            AddParam(cmd, "@Password", passwordHash);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())

            {
                var user = MapToUserDetails(reader);

                if (!user.IsActive)
                    throw new Exception("Օգտատերը գտնված չէ։");

                return user;
            }

            throw new Exception("Էլեկտրոնային հասցեն կամ գաղտնաբառը սխալ են։");
        }
       
 //DELETE ASYNC  
         public async Task<DeleteStatus> DeleteAsync(int id, int userId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var checkCmd = new SqlCommand("SELECT IsActive, DeletedBy FROM Users WHERE Id = @Id", conn);
            {
                checkCmd.Parameters.AddWithValue("@Id", id);

                using var reader = await checkCmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    return DeleteStatus.NotFound;

                bool isActive = (bool)reader["IsActive"];
                if (!isActive)
                    return DeleteStatus.AlreadyDeleted;

                reader.Close();

                using var updateCmd = new SqlCommand("UPDATE Users SET IsActive = 0, DeletedBy = @DeletedBy, " +
                                                     "ModifiedDate = GETDATE() WHERE Id = @Id", conn);

                updateCmd.Parameters.AddWithValue("@Id", id);
                updateCmd.Parameters.AddWithValue("@DeletedBy", userId);
                await updateCmd.ExecuteNonQueryAsync();

                return DeleteStatus.Deleted;
            }
        }
 
 // GET LIST ASYNC
        public async Task<IEnumerable<UserDetails>> GetListAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetAllUserFullJSON", conn) { CommandType = CommandType.StoredProcedure };
            await conn.OpenAsync();

            var json = await cmd.ExecuteScalarAsync() as string;

            if (string.IsNullOrEmpty(json)) return new List<UserDetails>();

            return JsonConvert.DeserializeObject<List<UserDetails>>(json);
        }

        public async Task<UserDetails> GetAsync(int userId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("SELECT * FROM Users WHERE Id = @UserId", conn);
            AddParam(cmd, "@UserId", userId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var user = MapToUserDetails(reader);

                if (!user.IsActive)
                    throw new Exception("Օգտատերը ակտիվ չէ։");

                return user;
            }

            throw new Exception("Օգտատերը գտնված չէ։");
        }
    }
}
