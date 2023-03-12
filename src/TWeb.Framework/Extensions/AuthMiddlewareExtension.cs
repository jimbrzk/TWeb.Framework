using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWeb.Framework.Middlewares;

namespace TWeb.Framework.Extensions
{
    public static class AuthMiddlewareExtension
    {
        public static IApplicationBuilder UseTWebAuth(this IApplicationBuilder app)
        {
            return app.UseMiddleware<TWebAuthorizationMiddleware>();
        }
    }
}
