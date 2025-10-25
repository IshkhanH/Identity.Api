using Identity.Core.Interfaces;
using Identity.Core.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Identity.DataLayer.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {

        private readonly string _connectionString;

        public RefreshTokenRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private static void AddParam(SqlCommand cmd, string name, object value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        public async Task<RefreshTokenDto> GetRefreshTokenAsync(string token)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SELECT * FROM RefreshTokens WHERE Token = @Token AND IsRevoked = 0", conn);

            AddParam(cmd, "@Token", token);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToRefreshTokenDto(reader);
            }
            return null;
        }

        public async Task<RefreshTokenDto> GetRevokedTokenAsync(string token)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SELECT * FROM RefreshTokens WHERE Token = @Token AND IsRevoked = 1", conn);

            AddParam(cmd, "@Token", token);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToRefreshTokenDto(reader);
            }
            return null;
        }

        public async Task RevokeAllUserTokensAsync(int userId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("UPDATE RefreshTokens SET IsRevoked = 1, RevokedAt = @RevokedAt " +
                                           "WHERE UserId = @UserId AND IsRevoked = 0", conn);
            AddParam(cmd, "@UserId", userId);
            AddParam(cmd, "@RevokedAt", DateTime.UtcNow);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("UPDATE RefreshTokens SET IsRevoked = 1, RevokedAt = @RevokedAt " +
                                           "WHERE Token = @Token", conn);
            AddParam(cmd, "@Token", token);
            AddParam(cmd, "@RevokedAt", DateTime.UtcNow);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task SaveRefreshTokenAsync(int userId, string token, DateTime expiresAt, string ip)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("INSERT INTO RefreshTokens (UserId, Token, ExpiresAt, CreatedAt, IsRevoked," +
                             "CreatedByIp) VALUES (@UserId, @Token, @ExpiresAt, @CreatedAt, @IsRevoked, @CreatedByIp)", conn);
            AddParam(cmd, "@UserId", userId);
            AddParam(cmd, "@Token", token);
            AddParam(cmd, "@ExpiresAt", expiresAt);
            AddParam(cmd, "@CreatedAt", DateTime.UtcNow);
            AddParam(cmd, "@IsRevoked", false);
            AddParam(cmd, "@CreatedByIp", ip);

            await cmd.ExecuteNonQueryAsync();
        }

        private static RefreshTokenDto MapToRefreshTokenDto(SqlDataReader reader)
        {
            return new RefreshTokenDto
            {
                Token = reader["Token"]?.ToString() ?? string.Empty,
                UserId = Convert.ToInt32(reader["UserId"]),
                ExpiresAt = Convert.ToDateTime(reader["ExpiresAt"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                IsRevoked = Convert.ToBoolean(reader["IsRevoked"]),
                RevokedAt = reader["RevokedAt"] != DBNull.Value ? Convert.ToDateTime(reader["RevokedAt"]) : (DateTime?)null,
                CreatedByIp = reader["CreatedByIp"]?.ToString()
            };
        }
    }
}
