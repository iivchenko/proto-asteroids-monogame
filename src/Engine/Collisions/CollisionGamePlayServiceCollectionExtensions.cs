using Engine;
using Engine.Collisions;
using Engine.Entities;
using Engine.Rules;
namespace Microsoft.Extensions.DependencyInjection
{
    public static class CollisionGamePlayServiceCollectionExtensions
    {
        public static IServiceCollection AddCollisions(this IServiceCollection services, uint priority)
        {
            services.AddSingleton<IGamePlaySystem, CollisionGamePlaySystem>(x => new CollisionGamePlaySystem(x.GetRequiredService<IEventPublisher>(), x.GetRequiredService<IWorld>(), x.GetRequiredService<ICollisionService>(), priority));
 
            return services;
        }
    }
}
