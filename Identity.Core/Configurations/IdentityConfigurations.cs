using System.Reflection;
using FluentValidation;
using Identity.Core.Configurations.Options;
using Identity.Core.Interfaces;
using Identity.Core.Services;
using Identity.Core.Services.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Core.Configurations
{
    public static class IdentityConfigurations
    {
        private static readonly Assembly _assembly = 
            typeof(IdentityConfigurations).Assembly;

        public static IServiceCollection ConfigureIdentityCore(
            IServiceCollection services,
            IConfiguration configuration
            )
        {
            services.Configure<JWTConfigurationOptions>(options => configuration.GetSection(nameof(JWTConfigurationOptions)).Bind(options));
            
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IIdentityService, IdentityService>();

            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            services.AddScoped<IClientService, ClientService>();

            services.AddValidatorsFromAssembly(_assembly);

            return services;
        }
    }
}
