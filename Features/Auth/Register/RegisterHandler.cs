using Microsoft.AspNetCore.Identity;
using VsaCqrsTemplate.Common.Abstractions;
using VsaCqrsTemplate.Common.Cqrs;
using VsaCqrsTemplate.Features.Auth.Login;
using VsaCqrsTemplate.Infrastructure.Persistence.Auth;

namespace VsaCqrsTemplate.Features.Auth.Register
{
    public sealed class RegisterHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) 
    : ICommandHandler<RegisterCommand, Result<RegisterResponse>>
    {
        public async Task<Result<RegisterResponse>> Handle(
        RegisterCommand command, CancellationToken cancellationToken)
        {
            var existingUser = await userManager.FindByEmailAsync(command.Email);

            if (existingUser is not null)
            {
                return Result<RegisterResponse>.Failure("Email already exists.");
            }
            const string defaultRole = "Staff";

            // Ensure role exists
            var roleExists = await roleManager.RoleExistsAsync(defaultRole);

            if (!roleExists)
            {
                return Result<RegisterResponse>.Failure("Invalid role.");
            }

            var user = new ApplicationUser
            {
                UserName = command.Email,
                FullName = command.FullName,
                Email = command.Email,
            };


            var createResult = await userManager.CreateAsync(
                user,
                command.Password
            );



            if (!createResult.Succeeded)
            {
                return Result<RegisterResponse>.Failure("Failed to create user.");
            }
            var roleResult = await userManager.AddToRoleAsync(
                user,
                defaultRole
            );

            if (!roleResult.Succeeded)
            {
                return Result<RegisterResponse>.Failure("Failed to assign role.");
            }

            return Result<RegisterResponse>.Success(new RegisterResponse("User registered successfully."));
        }

    }
}
