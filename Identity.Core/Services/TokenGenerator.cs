using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Identity.Core.Configurations.Options;
using Identity.Core.Interfaces;
using Identity.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Core.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly JWTConfigurationOptions _configs;

        public TokenGenerator(IOptions<JWTConfigurationOptions> configs)
        {
            _configs = configs.Value;
        }

        public string GenerateToken(int id, string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configs.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JWTCustomClaims.Email, email),
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim("nameid", id.ToString())
            };

            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken
            (
                issuer: _configs.ValidIssuer,
                audience: _configs.ValidAudience,
                notBefore: now,
                claims: claims,
                expires: now.AddMinutes(_configs.LifeTime),
                signingCredentials: credentials
            );

            var encoded = new JwtSecurityTokenHandler().WriteToken(token);
            return encoded;

        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
