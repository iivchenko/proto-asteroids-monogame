using Engine.Audio;
using Engine.Collisions;
using Engine.Content;
using Engine.Graphics;
using Engine.MonoGame;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MonoGameServiceCollectionExtensions
    {
        public static IServiceCollection AddMonoGameDrawSystem(this IServiceCollection services)
        {
            services.TryAddSingleton<MonoGameDrawSystem>();
            services.TryAdd(new ServiceDescriptor(typeof(IPainter), x => x.GetService<MonoGameDrawSystem>(), ServiceLifetime.Singleton));
            services.TryAdd(new ServiceDescriptor(typeof(IDrawSystemBatcher), x => x.GetService<MonoGameDrawSystem>(), ServiceLifetime.Singleton));
            services.TryAdd(new ServiceDescriptor(typeof(IFontService), x => x.GetService<MonoGameDrawSystem>(), ServiceLifetime.Singleton));

            return services;
        }

        public static IServiceCollection AddMonoGameAudioSystem(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptions<AudioSettings>()
                .Bind(configuration)
                .ValidateDataAnnotations();

            services.TryAddSingleton<MonoGameSoundSystem>();
            services.TryAdd(new ServiceDescriptor(typeof(IAudioPlayer), x => x.GetService<MonoGameSoundSystem>(), ServiceLifetime.Singleton));
            services.TryAdd(new ServiceDescriptor(typeof(IMusicPlayer), x => x.GetService<MonoGameSoundSystem>(), ServiceLifetime.Singleton));

            return services;
        }

        public static IServiceCollection AddMonoGameContentSystem(this IServiceCollection services, string content)
        {
            services.TryAddSingleton(x => new ContentRoot(content));
            services.TryAddSingleton<MonoGameContentProvider>();
            services.TryAddSingleton<MonoGameCollisionService>();
            services.TryAdd(new ServiceDescriptor(typeof(IContentProvider), x => x.GetService<MonoGameContentProvider>(), ServiceLifetime.Singleton));
            services.TryAdd(new ServiceDescriptor(typeof(ICollisionService), x => x.GetService<MonoGameCollisionService>(), ServiceLifetime.Singleton));

            return services;
        }
    }
}
