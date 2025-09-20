using FluentValidation;
using Identity.Core.Interfaces;
using Identity.Core.Models.Client;
using Microsoft.Extensions.DependencyInjection;
using Identity.DataLayer.Entities;
using System.Reflection;

namespace Identity.Core.Services
{
    public class ClientService : IClientService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IClientRepository _clientRepository;

        public ClientService(
            IServiceProvider serviceProvider,
            IClientRepository clientRepository
            )
        {
            _serviceProvider = serviceProvider;
            _clientRepository = clientRepository;
        }

        public async Task<int> CreateAsync(ClientCreate model)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<ClientCreate>>();
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                throw new InvalidDataException(nameof(model));
            }

            var id = await _clientRepository.CreateAsync(new Client
            {
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                Mail = model.Mail,
                Address = model.Address,
                Passport = model.Passport,
                SurName = model.SurName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                UserId = model.UserId,
            });

            return id;
        }

        public async Task UpdateAsync(int id, ClientUpdate model)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<ClientUpdate>>();
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                throw new InvalidDataException(nameof(model));
            }

            await _clientRepository.UpdateAsync(new Client
            { 
                Id = id,
                Mail = model.Mail,
                Address = model.Address,
                Passport = model.Passport,
                SurName = model.SurName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                UserId= model.UserId,
            });
        }

        public async Task DeleteAsync(int id)
        {
            await _clientRepository.DeleteAsync(id);
        }

        public async Task<ClientDetails> GetAsync(int id)
        {
            var data = await _clientRepository.GetAsync(id);
            return new ClientDetails
            {
                Address = data.Address,
                CreatedDate = data.CreatedDate,
                FirstName = data.FirstName,
                LastName = data.LastName,
                PhoneNumber = data.PhoneNumber,
                Mail = data.Mail,
                Id = id,
                ModifiedDate = data.ModifiedDate,
                Passport = data.Passport,
                SurName = data.SurName,
            };
        }

        public async Task<IEnumerable<ClientDetails>> GetListAsync()
        {
            var list = await _clientRepository.GetListAsync();
            return list.Select(data => new ClientDetails
            {
                Address = data.Address,
                CreatedDate = data.CreatedDate,
                FirstName = data.FirstName,
                LastName = data.LastName,
                PhoneNumber = data.PhoneNumber,
                Mail = data.Mail,
                Id = data.Id,
                ModifiedDate = data.ModifiedDate,
                Passport = data.Passport,
                SurName = data.SurName,
            }).ToList();
        }
    }
}
