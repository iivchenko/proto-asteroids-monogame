using Engine;
using Engine.Events;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventServiceCollectionExtensions
    {
        public static IServiceCollection AddGameEvents(this IServiceCollection services, IEnumerable<Assembly> assembliesToScan, uint priority)
        {
            services.TryAddSingleton<ServiceFactory>(x => x.GetServices);
            services.TryAddSingleton<EventGamePlaySystem>(x => new EventGamePlaySystem(x.GetRequiredService<ServiceFactory>(), priority));
            services.Add(new ServiceDescriptor(typeof(IGamePlaySystem), x => x.GetService<EventGamePlaySystem>(), ServiceLifetime.Singleton));
            services.TryAdd(new ServiceDescriptor(typeof(IEventPublisher), x => x.GetService<EventGamePlaySystem>(), ServiceLifetime.Singleton));                      

            assembliesToScan
                .SelectMany(assembly => assembly.DefinedTypes)
                .Where(type => !type.IsGenericType && !type.IsAbstract)
                .Where(type => type.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
                .SelectMany(type => 
                            type
                                .GetInterfaces()
                                .Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                                .Where(@interface => !@interface.IsGenericMethodParameter)
                                .Select(@interface => new { Interface = @interface, Implementation = type }))
                .Select(type => new ServiceDescriptor(type.Interface, type.Implementation, ServiceLifetime.Singleton))
                .Iter(services.Add);

            return services;
        }
    }
}
