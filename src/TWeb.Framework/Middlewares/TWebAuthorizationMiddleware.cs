using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Features;
using TWeb.Framework.Exceptions;
using System.Net;
using TWeb.Framework.Attributes;

namespace TWeb.Framework.Middlewares
{
    public class TWebAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUserManager _userManager;
        private readonly ILogger<TWebAuthorizationMiddleware> _logger;

        public TWebAuthorizationMiddleware(RequestDelegate next, IUserManager userManager, ILogger<TWebAuthorizationMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            CancellationToken ct = context?.RequestAborted ?? CancellationToken.None;
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var authAttr = endpoint?.Metadata?.FirstOrDefault(m => m is TWebAuthorizeAttribute) as TWebAuthorizeAttribute;

            if (!(authAttr?.Anonymouse ?? true))
            {
                try
                {
                    _logger.LogDebug($"Checking authorization for {context.Request.Path}");

                    var user = await _userManager.GetCurrentUserAsync(ct);
                    if (ct.IsCancellationRequested)
                    {
                        context.Abort();
                        return;
                    }

                    if (user == null)
                    {
                        _logger.LogDebug("User not logged in");
                        await WriteUnauthorizedAsync(context, ct);
                        return;
                    }
                }
                catch (UserLockedException)
                {
                    _logger.LogDebug("User is locked");
                    await WriteUnauthorizedAsync(context, ct);
                    return;
                }
                catch (UserNotFoundException)
                {
                    _logger.LogDebug("User not exist");
                    await WriteUnauthorizedAsync(context, ct);
                    return;
                }
            }

            await _next.Invoke(context);
        }

        private Task WriteUnauthorizedAsync(HttpContext context, CancellationToken ct = default)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return context.Response.WriteAsync("Unauthorized", ct);
        }
    }
}
