using FluentValidation;
using Microsoft.AspNetCore.Identity;
using VsaCqrsTemplate.Common.Abstractions;
using VsaCqrsTemplate.Common.Cqrs;
using VsaCqrsTemplate.Features.Auth.Me;
using VsaCqrsTemplate.Infrastructure.Persistence.Auth;

namespace VsaCqrsTemplate.Features.Auth.Login
{
    public sealed class LoginHandler(
    UserManager<ApplicationUser> userManager,
    IJwtService jwt)
    : ICommandHandler<LoginCommand, Result<LoginResponse>>
    {
        public async Task<Result<LoginResponse>> Handle(
            LoginCommand command,
            CancellationToken ct)
        {
            var user = await userManager.FindByEmailAsync(command.Email);

            if (user is null)
                return Result<LoginResponse>.Failure("Credentials are invalid.");

            var isPasswordValid = await userManager.CheckPasswordAsync(
                user,
                command.Password);

            if (!isPasswordValid)
                return Result<LoginResponse>.Failure("Credentials are invalid.");

            var roles = await userManager.GetRolesAsync(user);

            if (roles.Count == 0)
                return Result<LoginResponse>.Failure("User has no assigned role.");

            var accessToken = await jwt.GenerateAccessTokenAsync(user);
            var refreshToken = await jwt.GenerateRefreshTokenAsync(user.Id);

            var response = new LoginResponse(
                AccessToken: accessToken.Token,
                ExpiresIn: (int)(accessToken.Expires - DateTime.UtcNow).TotalSeconds,
                ExpiresAt: accessToken.Expires,
                RefreshToken: refreshToken.Token,
                User: new MeResponse(
                    Id: user.Id,
                    Name: user.FullName ?? string.Empty,
                    Email: user.Email ?? string.Empty,
                    Role: roles[0]));

            return Result<LoginResponse>.Success(response);
        }
    }
}
