using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Identity.Core.Interfaces;
using Identity.Core.Models.Client;
using Identity.Core.Models.User;
using Identity.Core.Services.Repositories;
using Identity.DataLayer.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserRepository _userRepository;

        public UserService(
            IServiceProvider serviceProvider,
            IUserRepository clientRepository
            )
        {
            _serviceProvider = serviceProvider;
            _userRepository = clientRepository;
        }
        public async Task<int> CreateAsync(UserCreate model)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<UserCreate>>();
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                // change
                throw new InvalidDataException(nameof(model));
            }

            var id = await _userRepository.CreateAsync(new User
            {
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                Email = model.Email,
                Password = model.Password,
            });
            return id;
        }
        public async Task UpdateAsync(int id, UserUpdate model)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<UserUpdate>>();
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                // change
                throw new InvalidDataException(nameof(model));
            }

            await _userRepository.UpdateAsync(new User
            {
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                Email = model.Email,
                Password = model.Password,
            });
        }

        public async Task DeleteAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task<UserDetails> GetAsync(int id)
        {
            var data = await _userRepository.GetAsync(id);
            return new UserDetails
            {
                CreatedDate = data.CreatedDate,
                ModifiedDate = data.ModifiedDate,
                Email = data.Email,
                Password = data.Password,
                Id = data.Id
            };

        }

        public async Task<IEnumerable<UserDetails>> GetListAsync()
        {
            var list = await _userRepository.GetListAsync();
            return list.Select(data => new UserDetails
            {
                CreatedDate = data.CreatedDate,
                ModifiedDate = data.ModifiedDate,
                Email = data.Email,
                Password = data.Password,
                Id= data.Id
                                
            }).ToList();
        }

        
    }
}
