using FluentValidation;
using VsaCqrsTemplate.Common.Abstractions;
using VsaCqrsTemplate.Common.Cqrs;

namespace VsaCqrsTemplate.Features.Auth.Register
{
    public sealed class RegisterEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/auth/register", async 
            (
                RegisterCommand command,
                IValidator<RegisterCommand> validator,
                ICommandHandler<RegisterCommand, 
                Result<RegisterResponse>> handler, 
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
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
            })
            .WithName("Register")
            .WithTags("Auth");
        }
    }
}
