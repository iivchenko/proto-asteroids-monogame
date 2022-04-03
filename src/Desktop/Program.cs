using Comora;
using Core.Entities;
using Core.Leaderboards;
using Core.Screens;
using Core.Screens.GamePlay;
using Core.Screens.GamePlay.Systems;
using Engine;
using Engine.Entities;
using Engine.Graphics;
using Engine.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace Desktop
{
    public static class Program
    {
        private const string ConfigFile = "game-settings.json";
        private const string LeaderBoardsFile = "leaders.json";

        [STAThread]
        public static void Main()
        {
#if MacOS
            var root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources");
#else
            var root = string.Empty;
#endif
            var configFilePath = Path.Combine(root, ConfigFile);
            var leaderboardsFilePath = Path.Combine(root, LeaderBoardsFile);
            var contentRoot = root;
            GameBuilder
                .CreateBuilder()
                .WithServices(container =>
                    {
                        var configuration =
                            new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile(configFilePath, optional: true, reloadOnChange: true)
                                .Build();

                        container
                            .AddOptions()
                            .Configure<GameSettings>(configuration)
                            .AddSingleton<IRepository<GameSettings>>(_ => new JsonRepository<GameSettings>(configFilePath))
                            .Decorate<IRepository<GameSettings>>(x => new DefaultInitializerRepositoryDecorator<GameSettings>(x, new GameSettings { Audio = { SfxVolume = 0.2f, MusicVolume = 0.2f } }))
                            .AddSingleton<IRepository<Collection<LeaderboardItem>>>(_ => new JsonRepository<Collection<LeaderboardItem>>(leaderboardsFilePath))
                            .Decorate<IRepository<Collection<LeaderboardItem>>>(x => new DefaultInitializerRepositoryDecorator<Collection<LeaderboardItem>>(x, new Collection<LeaderboardItem>()))
                            .AddSingleton<IEntityFactory, EntityFactory>()
                            .AddSingleton<IProjectileFactory, ProjectileFactory>()
                            .AddSingleton<IViewport, Viewport>(_ => new Viewport(0.0f, 0.0f, 3840.0f, 2160.0f))
                            .AddSingleton<ICamera, Camera>()
                            .AddMonoGameContentSystem(contentRoot)
                            .AddMonoGameDrawSystem()
                            .AddMonoGameAudioSystem(configuration.GetSection("Audio"))
                            .AddSingleton<IGamePlaySystem, OutOfScreenSystem>()
                            .AddEntitySystem(10, 50)
                            .AddCollisions(20)
                            .AddSingleton<IGamePlaySystem, UfoAiSystem>(x => new UfoAiSystem(x.GetRequiredService<IWorld>(), 10))
                            .AddGameRules(new[] { Assembly.GetAssembly(typeof(Core.Version)) }, 30)
                            .AddSingleton<GamePlayContext>()
                            .AddSingleton<LeaderboardsManager>();
                    })
                .WithConfiguration(config => // TODO: This beast seems become redundant
                    {
                        config.FullScreen = false; // TODO: Make as a part of graphics settings?
                        config.IsMouseVisible = false; // TODO: Make as a part of input settings?
                        config.ContentPath = "Content"; // TODO: Make as a part of content settings?
                        config.ScreenColor = Colors.Black; // TODO: Make as a part of graphics settings?
                    })
                .Build((services, config) => new MonoGameGame(services, config, new BootstrapScreen<MainMenuScreen>()))
                .Run();
        }
    }
}
