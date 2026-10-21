using FluentValidation;
using System.Reflection.Metadata;
using VsaCqrsTemplate.Common.Abstractions;
using VsaCqrsTemplate.Common.Cqrs;

namespace VsaCqrsTemplate.Features.Auth.Login
{
   
    public sealed class LoginEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/auth/login", async (
            LoginCommand command,
            IValidator<LoginCommand> validator,
            ICommandHandler<LoginCommand, Result<LoginResponse>> handler,
            CancellationToken ct) =>
            {

                var validationResult = await validator.ValidateAsync(command, ct);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );

                    return Results.BadRequest(new
                    {
                        message = "Validation failed.",
                        errors
                    });
                }

                var result = await handler.Handle(command, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.Json(
                        new { message = result.Error },
                        statusCode: StatusCodes.Status401Unauthorized);
            })
        .WithName("Login")
        .WithTags("Auth");
        }
    }
}
