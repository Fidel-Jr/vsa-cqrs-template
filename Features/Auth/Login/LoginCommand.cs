using VsaCqrsTemplate.Common.Abstractions;
using VsaCqrsTemplate.Common.Cqrs;
using VsaCqrsTemplate.Features.Auth.Me;

namespace VsaCqrsTemplate.Features.Auth.Login
{
    public sealed record LoginCommand(string Email, string Password) : ICommand<Result<LoginResponse>>;

    public sealed record LoginResponse(string AccessToken, int ExpiresIn, DateTime? ExpiresAt, string RefreshToken, MeResponse User);

}