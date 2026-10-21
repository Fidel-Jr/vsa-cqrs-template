using VsaCqrsTemplate.Infrastructure.Persistence.Auth;

namespace VsaCqrsTemplate.Common.Abstractions
{
    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
    }
    public interface IJwtService
    {
        Task<TokenResponse> GenerateAccessTokenAsync(ApplicationUser user);
        Task<TokenResponse> GenerateRefreshTokenAsync(string userId);
    }
}
