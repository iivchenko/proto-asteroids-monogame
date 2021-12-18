using KenneyAsteroids.Engine;
using KenneyAsteroids.Engine.Collisions;
using KenneyAsteroids.Engine.Entities;
using KenneyAsteroids.Engine.Rules;
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
