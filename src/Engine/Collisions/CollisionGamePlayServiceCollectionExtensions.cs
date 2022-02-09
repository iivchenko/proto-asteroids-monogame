using Engine;
using Engine.Collisions;
using Engine.Entities;
using Engine.Rules;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CollisionGamePlayServiceCollectionExtensions
    {
        public static IServiceCollection AddCollisions(this IServiceCollection services, uint priority)
        {
            services.AddSingleton<IGamePlaySystem, CollisionGamePlaySystem>(x => new CollisionGamePlaySystem(x.GetRequiredService<IEventPublisher>(), x.GetRequiredService<IWorld>(), priority));
 
            return services;
        }
    }
}
