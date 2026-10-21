using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using VsaCqrsTemplate.Common.Cqrs;

namespace VsaCqrsTemplate.Common.Extensions
{
    public static class CqrsRegistrationExtensions
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false });

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();

                foreach (var i in interfaces)
                {
                    if (!i.IsGenericType)
                        continue;

                    var openInterface = i.GetGenericTypeDefinition();

                    if (openInterface == typeof(ICommandHandler<,>) ||
                        openInterface == typeof(ICommandHandler<>) ||
                        openInterface == typeof(IQueryHandler<,>))
                    {
                        services.TryAddScoped(i, type);
                    }
                }
            }
            return services;
        }
    }
}
