using KenneyAsteroids.Engine;
using KenneyAsteroids.Engine.Entities;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EntitiesGamePlayServiceCollectionExtensions
    {
        public static IServiceCollection AddEntitySystem(this IServiceCollection services, uint prePriority, uint postPriority)
        {
            services.TryAddSingleton<IWorld, World>();
            services.AddSingleton<IGamePlaySystem, EntityPreSystem>(x => new EntityPreSystem(x.GetRequiredService<IWorld>(), prePriority));
            services.AddSingleton<IGamePlaySystem, EntityPostSystem>(x => new EntityPostSystem(x.GetRequiredService<IWorld>(), postPriority));

            return services;
        }
    }
}
