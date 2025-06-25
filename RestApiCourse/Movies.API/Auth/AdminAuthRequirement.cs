using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Movies.API.Auth
{
    public class AdminAuthRequirement : IAuthorizationHandler, IAuthorizationRequirement
    {
        private readonly string _apiKey;

        public AdminAuthRequirement(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (context.User.HasClaim( AuthConstants.AdminUserClaimName, "true"))
            {
                context.Succeed(this);
                return Task.CompletedTask;
            }
            
            var httpContext = context.Resource as HttpContext;
            if (httpContext is null)
            {
                return Task.CompletedTask;
            }

            if (!httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey) || extractedApiKey != _apiKey)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (_apiKey != extractedApiKey)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var identity = (ClaimsIdentity)context.User.Identity!;
            identity.AddClaim(new Claim("userId", Guid.Parse("b59eba81-f72d-488a-ab0b-40c519539f17").ToString()));
            context.Succeed(this);
            return Task.CompletedTask;
        }
    }
}
