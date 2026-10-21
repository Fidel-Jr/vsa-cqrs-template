using System.Security.Claims;
using VsaCqrsTemplate.Common.Abstractions;

namespace VsaCqrsTemplate.Infrastructure.Persistence.Auth
{
    public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor)
    : ICurrentUser
    {
        private ClaimsPrincipal? User =>
            httpContextAccessor.HttpContext?.User;

        public string? UserId
        {
            get
            {
                var value = User?.FindFirstValue(ClaimTypes.NameIdentifier);

                return string.IsNullOrEmpty(value) ? null : value;
            }
        }

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated ?? false;
    }
}
