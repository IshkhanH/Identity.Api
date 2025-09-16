using System.Data;
using System.Reflection;
using FluentValidation;
using Identity.Core.Interfaces;
using Identity.Core.Services;
using Identity.Core.Services.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Identity.Core.Configurations
{
    public static class IdentityConfigurationsExtensions
    {
        private static readonly Assembly _assembly = 
            typeof(IdentityConfigurationsExtensions).Assembly;

        public static IServiceCollection ConfigureIdentityCore(
            this IServiceCollection services,
            IConfiguration configuration
            )
        {
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IClientService, ClientService>();

            services.AddScoped<IUserRepository, UserRepositօry>();
            services.AddScoped<IUserService, UserService>();


            services.AddValidatorsFromAssembly(_assembly);

            return services;
        }
    }
}
