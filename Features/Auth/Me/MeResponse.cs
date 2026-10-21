namespace VsaCqrsTemplate.Features.Auth.Me
{
    public sealed record MeResponse(string Id, string Name, string Email, string Role);
}
