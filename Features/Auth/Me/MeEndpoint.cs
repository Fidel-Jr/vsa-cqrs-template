using VsaCqrsTemplate.Common.Abstractions;
using VsaCqrsTemplate.Common.Cqrs;

namespace VsaCqrsTemplate.Features.Auth.Me
{
    public sealed class MeEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/auth/me", async (IQueryHandler<MeQuery, Result<MeResponse>> handler, CancellationToken ct) =>
            {
                var result = await handler.Handle(new MeQuery(), ct);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.Json(
                        new { message = result.Error },
                        statusCode: StatusCodes.Status401Unauthorized);
            })
            .WithName("Me")
            .WithTags("Auth");
        }
    }
}
