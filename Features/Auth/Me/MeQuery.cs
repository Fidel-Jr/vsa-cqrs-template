using VsaCqrsTemplate.Common.Abstractions;
using VsaCqrsTemplate.Common.Cqrs;

namespace VsaCqrsTemplate.Features.Auth.Me
{
    public sealed class MeQuery : IQuery<Result<MeResponse>>;

}
