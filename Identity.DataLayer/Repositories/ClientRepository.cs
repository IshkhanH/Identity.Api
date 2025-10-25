using Identity.Core.Enums;
using Identity.Core.Interfaces;
using Identity.Core.Models.Client;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Text;


namespace Identity.DataLayer.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private static void AddParam(SqlCommand cmd, string name, object value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        // CREATE ASYNC
        public async Task<int> CreateAsync(ClientCreate client)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using (var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Users WHERE id = @UserId", conn))
            {
                checkCmd.Parameters.AddWithValue("@UserId", client.UserId);
                var exists = (int)await checkCmd.ExecuteScalarAsync();
                if (exists == 0)
                {
                    throw new InvalidDataException($"{client.UserId} UserId գոյություն չունի։");
                }
            }

            foreach (var email in client.Emails)
            {
                using var checkEmailCmd = new SqlCommand("SELECT COUNT(1) FROM ClientEmails WHERE Email = @Email", conn);
                checkEmailCmd.Parameters.AddWithValue("@Email", email.Email);
                var emailExists = (int)await checkEmailCmd.ExecuteScalarAsync();
                if (emailExists > 0)
                {
                    throw new InvalidDataException($"{email.Email} էլեկտրոնային հասցեն արդեն գրանցված է։");
                }
            }

            foreach (var doc in client.Documents)
            {
                using var checkPassportCmd = new SqlCommand(
                "SELECT COUNT(1) FROM ClientDocuments WHERE Passport = @Passport", conn);
                checkPassportCmd.Parameters.AddWithValue("@Passport", doc.Passport);
                var passportExists = (int)await checkPassportCmd.ExecuteScalarAsync();
                if (passportExists > 0)
                {
                    throw new InvalidDataException($"{doc.Passport} Passport-ն արդեն գրանցված է։");
                }


                using var checkIdCardCmd = new SqlCommand(
                "SELECT COUNT(1) FROM ClientDocuments WHERE IdCard = @IdCard", conn);
                checkIdCardCmd.Parameters.AddWithValue("@IdCard", doc.IdCard);
                var idCardExists = (int)await checkIdCardCmd.ExecuteScalarAsync();
                if (idCardExists > 0)
                {
                    throw new InvalidDataException($"{doc.IdCard} IdCard-ը արդեն գրանցված է։");
                }
            }

            using var cmd = new SqlCommand("sp_AddClient", conn) { CommandType = CommandType.StoredProcedure };

            AddParam(cmd, "@FirstName", client.FirstName);
            AddParam(cmd, "@LastName", client.LastName);
            AddParam(cmd, "@SurName", client.SurName);
            AddParam(cmd, "@Address", client.Address);
            AddParam(cmd, "@UserId", client.UserId);
            AddParam(cmd, "@Emails", JsonConvert.SerializeObject(client.Emails));
            AddParam(cmd, "@Phones", JsonConvert.SerializeObject(client.Phones));
            AddParam(cmd, "@Documents", JsonConvert.SerializeObject(client.Documents));

            var Id = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(Id);
        }

        // GET ASYNC
        public async Task<ClientDetails> GetAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetClientFullJSON_ById", conn) { CommandType = CommandType.StoredProcedure };
            AddParam(cmd, "@ClientId", id);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var json = reader[0].ToString();
                return JsonConvert.DeserializeObject<ClientDetails>(json);
            }

            return null;
        }

        //GET LIST ASYNC
        public async Task<IEnumerable<ClientDetails>> GetListAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetAllClientFullJSON", conn) { CommandType = CommandType.StoredProcedure };
            await conn.OpenAsync();
            var sb = new StringBuilder();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    sb.Append(reader.GetString(0));
                }
            }
            var json = sb.ToString();
            if (string.IsNullOrEmpty(json)) return new List<ClientDetails>();

            return JsonConvert.DeserializeObject<List<ClientDetails>>(json);
        }

        // UPDATE ASYNC
        public async Task<Core.Enums.UpdateStatus> UpdateAsync(ClientUpdate client)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var checkCmd = new SqlCommand("SELECT IsDeleted FROM Clients WHERE Id = @Id", conn);

            checkCmd.Parameters.AddWithValue("@Id", client.Id);

            var result = await checkCmd.ExecuteScalarAsync();
            if (result == null)
                return Core.Enums.UpdateStatus.NotFound;

            bool isDeleted = (bool)result;

            if (isDeleted)
                return Core.Enums.UpdateStatus.Deleted;

            foreach (var email in client.Emails)
            {
                using var checkEmailCmd = new SqlCommand(
                "SELECT COUNT(1) FROM ClientEmails WHERE Email = @Email AND ClientId <> @ClientId", conn);
                checkEmailCmd.Parameters.AddWithValue("@Email", email.Email);
                checkEmailCmd.Parameters.AddWithValue("@ClientId", client.Id);

                var emailExists = (int)await checkEmailCmd.ExecuteScalarAsync();
                if (emailExists > 0)
                {
                    throw new InvalidDataException($"{email.Email} էլեկտրոնային հասցեն արդեն օգտագործվում է այլ հաճախորդի կողմից։");
                }
            }

            foreach (var doc in client.Documents)
            {

                using var checkPassportCmd = new SqlCommand(
                    "SELECT COUNT(1) FROM ClientDocuments WHERE Passport = @Passport AND ClientId <> @ClientId", conn);
                checkPassportCmd.Parameters.AddWithValue("@Passport", doc.Passport);
                checkPassportCmd.Parameters.AddWithValue("@ClientId", client.Id);
                var passportExists = (int)await checkPassportCmd.ExecuteScalarAsync();
                if (passportExists > 0)
                {
                    throw new InvalidDataException($"{doc.Passport} Passport-ն արդեն գրանցված է։");
                }

                using var checkIdCardCmd = new SqlCommand(
                    "SELECT COUNT(1) FROM ClientDocuments WHERE IdCard = @IdCard AND ClientId <> @ClientId", conn);
                checkIdCardCmd.Parameters.AddWithValue("@IdCard", doc.IdCard);
                checkIdCardCmd.Parameters.AddWithValue("@ClientId", client.Id);
                var idCardExists = (int)await checkIdCardCmd.ExecuteScalarAsync();
                if (idCardExists > 0)
                {
                    throw new InvalidDataException($"{doc.IdCard} IdCard-ը արդեն գրանցված է։");
                }

            }

            using var cmd = new SqlCommand("sp_UpdateClientFull", conn) { CommandType = CommandType.StoredProcedure };

            AddParam(cmd, "@ClientId", client.Id);
            AddParam(cmd, "@FirstName", client.FirstName);
            AddParam(cmd, "@LastName", client.LastName);
            AddParam(cmd, "@SurName", client.SurName);
            AddParam(cmd, "@Address", client.Address);
            AddParam(cmd, "@UserId", client.UserId);
            AddParam(cmd, "@ModifiedBy", client.UserId);
            AddParam(cmd, "@Phones", JsonConvert.SerializeObject(client.Phones));
            AddParam(cmd, "@Emails", JsonConvert.SerializeObject(client.Emails));
            AddParam(cmd, "@Documents", JsonConvert.SerializeObject(client.Documents));

            await cmd.ExecuteNonQueryAsync();

            return Core.Enums.UpdateStatus.Updated;
        }

        //DELETE ASYNC
        public async Task<DeleteStatus> DeleteAsync(int id, int userId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var checkCmd = new SqlCommand("SELECT IsDeleted, DeletedBy FROM Clients WHERE Id = @Id", conn);
            {
                checkCmd.Parameters.AddWithValue("@Id", id);

                using var reader = await checkCmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    return DeleteStatus.NotFound;

                bool isDeleted = (bool)reader["IsDeleted"];
                if (isDeleted)
                    return DeleteStatus.AlreadyDeleted;

                reader.Close();

                using var updateCmd = new SqlCommand(
                "UPDATE Clients SET IsDeleted = 1,DeletedBy = @DeletedBy, ModifiedDate = GETDATE() WHERE Id = @Id", conn);
                updateCmd.Parameters.AddWithValue("@DeletedBy", userId);
                updateCmd.Parameters.AddWithValue("@Id", id);
                await updateCmd.ExecuteNonQueryAsync();

                return DeleteStatus.Deleted;
            }
        }

        //GET BY USERID ASYNC
        public async Task<IEnumerable<ClientDetails>> GetByUserIdAsync(int userId)
        {
            using var conn = new SqlConnection(_connectionString);
            using (var checkUserCmd = new SqlCommand("SELECT COUNT(1) FROM Users WHERE Id = @UserId", conn))
            {
                checkUserCmd.Parameters.AddWithValue("@UserId", userId);

                await conn.OpenAsync();

                var userExists = (int)await checkUserCmd.ExecuteScalarAsync() > 0;
                if (!userExists)
                {
                    throw new InvalidDataException();
                }
            }
            using var cmd = new SqlCommand("sp_GetClientsByUserIdFullJSON", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UserId", userId);

            var json = await cmd.ExecuteScalarAsync() as string;

            if (string.IsNullOrEmpty(json))
                return new List<ClientDetails>();

            return JsonConvert.DeserializeObject<List<ClientDetails>>(json);
        }
    }
}
