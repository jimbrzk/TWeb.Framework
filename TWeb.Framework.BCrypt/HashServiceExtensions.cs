using Microsoft.Extensions.DependencyInjection;
using TWeb.Framework.DAL;
using TWeb.Framework.Services;

namespace TWeb.Framework.BCrypt
{
    public static class HashServiceExtensions
    {
        public static IServiceCollection UseTWebBCryptHashing(this IServiceCollection services)
        {
            return services.AddSingleton<ISecretHashingService, BCryptHashingService>();
        }
    }
}