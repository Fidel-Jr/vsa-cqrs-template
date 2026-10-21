using MySqlX.XDevAPI.Common;
using System.Windows.Input;
using VsaCqrsTemplate.Common.Abstractions;
using VsaCqrsTemplate.Common.Cqrs;

namespace VsaCqrsTemplate.Features.Auth.Register
{
    public sealed record RegisterCommand(string FullName, string Email, string Password) : ICommand<Result<RegisterResponse>>;

    public sealed record RegisterResponse(string message);
}
