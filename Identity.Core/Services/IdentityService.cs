using FluentValidation;
using Identity.Core.Helpers;
using Identity.Core.Interfaces;
using Identity.Core.Models.User;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Core.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITokenGenerator _tokenGenerator;

        public IdentityService(
            IUserRepository userRepository,
            IServiceProvider serviceProvider,
            ITokenGenerator tokenGenerator
            )
        {
            _userRepository = userRepository;
            _serviceProvider = serviceProvider;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<LoginRequest>>();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new Exception();
            }

            var passwrodHash = HashHelper.GetHash(request.Password, request.Email);
            var user = await _userRepository.GetAsync(request.Email, passwrodHash);

            return new AuthResponse
            {
                Token = _tokenGenerator.GenerateToken(user.Id, request.Email)
            };
        }

        public async Task<AuthResponse> RegistrationAsync(RegistrationRequest request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<RegistrationRequest>>();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new Exception();
            }

            var passwrodHash = HashHelper.GetHash(request.Password, request.Email);
            var userId = await _userRepository.CreateAsync(new DataLayer.Entities.User
            {
                Password = passwrodHash,
                Email = request.Email,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            });

            return new AuthResponse
            {
                Token = _tokenGenerator.GenerateToken(userId, request.Email)
            };
        }
    }
}
