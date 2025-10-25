using System.Net;
using FluentValidation;
using Identity.Core.Configurations.Options;
using Identity.Core.Enums;
using Identity.Core.Helpers;
using Identity.Core.Interfaces;
using Identity.Core.Models.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace Identity.Core.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly JWTConfigurationOptions _jwtOptions;

        public IdentityService(
            IUserRepository userRepository,
            IServiceProvider serviceProvider,
            ITokenGenerator tokenGenerator,
            IRefreshTokenRepository refreshTokenRepository,
            IOptions<JWTConfigurationOptions> jwtOptions
            )
        {
            _userRepository = userRepository;
            _serviceProvider = serviceProvider;
            _tokenGenerator = tokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, string ipAddress)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<LoginRequest>>();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new Exception(string.Join(", ", errors));
            }

            var passwrodHash = HashHelper.GetHash(request.Password, request.Email);
            var user = await _userRepository.GetAsync(request.Email, passwrodHash) ?? throw new Exception
                                                     ("Էլեկտրոնային հասցեն կամ գաղտնաբառը սխալ են:");
            if (!user.IsActive)
            {
                throw new Exception("Օգտատերը ակտիվ չէ։");
            }

            return await GenerateAuthResponse(user.Id, request.Email, ipAddress ?? "Unknown");

            var refreshTokenString = _tokenGenerator.GenerateRefreshToken();

            await _refreshTokenRepository.SaveRefreshTokenAsync(user.Id,refreshTokenString,DateTime.UtcNow.AddDays(7),ipAddress);

            var accessToken = _tokenGenerator.GenerateToken(user.Id, user.Email);

            return new AuthResponse
            {
                Token = accessToken,
                RefreshToken = refreshTokenString
            };
        }

        public async Task<AuthResponse> RegistrationAsync(RegistrationRequest request, string ipAddress)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<RegistrationRequest>>();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new Exception(string.Join(", ", errors));
            }

            request.Password = HashHelper.GetHash(request.Password, request.Email);

            var userId = await _userRepository.CreateAsync(request);

            return await GenerateAuthResponse(userId, request.Email, ipAddress ?? "Unknown");

        }
        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, string ipAddress)
        {
            var tokenData = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken) ?? throw new Exception
                                                                              ("Անվավեր refresh token։");

            if (tokenData.ExpiresAt < DateTime.UtcNow)
                throw new Exception("Refresh token-ը ժամկետանց է։");

            await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken);

            var user = await _userRepository.GetAsync(tokenData.UserId);

            if (user == null || !user.IsActive)
                throw new Exception("Օգտատերը գտնված չէ կամ ակտիվ չէ։");

            return await GenerateAuthResponse(user.Id, user.Email, ipAddress);
        }
        public async Task RevokeTokenAsync(string refreshToken)
        {
            var tokenData = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken) ?? throw new Exception
                                                                              ("Token-ը գտնված չէ։");
            await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken);
        }
        public async Task RevokeAllUserTokensAsync(int userId)
        {
            await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
        }

        public async Task<DeleteStatus> DeleteAsync(int id, int userId)
        {
            return await _userRepository.DeleteAsync(id, userId);
        }

        public async Task<IEnumerable<UserDetails>> GetListAsync()
        {
            var list = await _userRepository.GetListAsync();
            return list.Select(data => new UserDetails
            {
                CreatedDate = data.CreatedDate,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Id = data.Id,
                ModifiedDate = data.ModifiedDate,
                SurName = data.SurName,
                Email = data.Email,
                IsActive = data.IsActive,
                DeletedBy = data.DeletedBy,
            }).ToList();
        }
        private async Task<AuthResponse> GenerateAuthResponse(int userId, string email, string ipAddress)
        {
            var accessToken = _tokenGenerator.GenerateToken(userId, email);

            string refreshToken = null;
            DateTime? refreshTokenExpiresAt = null;

            if (_jwtOptions.EnableRefreshToken)
            {
                refreshToken = _tokenGenerator.GenerateRefreshToken();
                refreshTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshTokenLifeTime);

                await _refreshTokenRepository.SaveRefreshTokenAsync(userId, refreshToken, refreshTokenExpiresAt.Value, ipAddress);
            }

            return new AuthResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.LifeTime),
                RefreshTokenExpiresAt = refreshTokenExpiresAt ?? DateTime.UtcNow
            };
        }

    }

}





