using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TWeb.Framework.DAL;
using TWeb.Framework.Services;

namespace TWeb.Framework.Extensions
{
    public static class UserManagerServiceExtension
    {
        public static IServiceCollection AddTWebUserManager(this IServiceCollection services) => AddTWebUserManager<SecretHashingService>(services); 

        public static IServiceCollection AddTWebUserManager<TSecretHashingService>(this IServiceCollection services) where TSecretHashingService : class, ISecretHashingService
        {
            if (!services.Any(x => x.ImplementationInstance is IHttpContextAccessor))
                throw new Exception("You need to implement IHttpContextAccessor");

            return services
                .AddSingleton<ISecretHashingService, TSecretHashingService>()
                .AddScoped<IUserManager, UserManager>();
        }
    }
}
