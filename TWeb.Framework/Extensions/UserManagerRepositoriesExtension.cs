using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TWeb.Framework.DAL;
using TWeb.Framework.Services;

namespace TWeb.Framework.Extensions
{
    public static class UserManagerRepositoriesExtension
    {
        public static IServiceCollection AddTWebUserManagerRepositories<TUserRepository, TUserTokenRepository, TApiKeyRepository>(this IServiceCollection services) 
            where TUserRepository : class, IUsersRepository
            where TUserTokenRepository : class, IUserTokenRepository
            where TApiKeyRepository : class, IApiKeysRepository
        {
            return services
                .AddTransient<IUsersRepository, TUserRepository>()
                .AddTransient<IUserTokenRepository, TUserTokenRepository>()
                .AddTransient<IApiKeysRepository, TApiKeyRepository>();
        }
    }
}
