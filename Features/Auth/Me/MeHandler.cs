using Microsoft.AspNetCore.Identity;
using VsaCqrsTemplate.Common.Abstractions;
using VsaCqrsTemplate.Common.Cqrs;
using VsaCqrsTemplate.Infrastructure.Persistence.Auth;

namespace VsaCqrsTemplate.Features.Auth.Me
{
    public sealed class MeHandler(ICurrentUser currentUser, UserManager<ApplicationUser> userManager) : IQueryHandler<MeQuery, Result<MeResponse>>
    {
        public async Task<Result<MeResponse>> Handle(MeQuery query, CancellationToken ct)
        {
            var userId = currentUser.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Result<MeResponse>.Failure("User is not authenticated.");
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<MeResponse>.Failure("User not found.");
            }
            var response = new MeResponse(
                Id: user.Id,
                Name: user.UserName,
                Email: user.Email,
                Role: (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty);
            return Result<MeResponse>.Success(response);
        }
    }
}
