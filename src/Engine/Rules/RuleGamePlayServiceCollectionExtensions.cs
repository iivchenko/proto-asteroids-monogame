using Engine;
using Engine.Rules;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RuleGamePlayServiceCollectionExtensions
    {
        public static IServiceCollection AddGameRules(this IServiceCollection services, IEnumerable<Assembly> assembliesToScan, uint priority)
        {
            services.TryAddSingleton<ServiceFactory>(x => x.GetServices);
            services.TryAddSingleton<RuleGamePlaySystem>(x => new RuleGamePlaySystem(x.GetRequiredService<ServiceFactory>(), priority));
            services.Add(new ServiceDescriptor(typeof(IGamePlaySystem), x => x.GetService<RuleGamePlaySystem>(), ServiceLifetime.Singleton));
            services.TryAdd(new ServiceDescriptor(typeof(IEventPublisher), x => x.GetService<RuleGamePlaySystem>(), ServiceLifetime.Singleton));                      

            assembliesToScan
                .SelectMany(assembly => assembly.DefinedTypes)
                .Where(type => !type.IsGenericType && !type.IsAbstract)
                .Where(type => type.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IRule<>)))
                .SelectMany(type => 
                            type
                                .GetInterfaces()
                                .Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IRule<>))
                                .Where(@interface => !@interface.IsGenericMethodParameter)
                                .Select(@interface => new { Interface = @interface, Implementation = type }))
                .Select(type => new ServiceDescriptor(type.Interface, type.Implementation, ServiceLifetime.Singleton))
                .Iter(services.Add);

            return services;
        }
    }
}
