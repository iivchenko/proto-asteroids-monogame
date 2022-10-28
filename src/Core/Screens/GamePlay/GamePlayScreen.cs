using Core.Entities;
using Engine;
using Engine.Audio;
using Engine.Content;
using Engine.Entities;
using Engine.Graphics;
using Engine.Events;
using Engine.Screens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Numerics;

using XTime = Microsoft.Xna.Framework.GameTime;
using System;
using System.Collections.Generic;
using Core.Screens.GamePlay.PlayerControllers;

namespace Core.Screens.GamePlay
{
    public sealed class GamePlayScreen : GameScreen
    {
        private List<IGamePlaySystem> _systems;
        private IWorld _world;
        private IViewport _viewport;
        private IEventPublisher _publisher;
        private IMusicPlayer _musicPlayer;
        private IPlayerController _controller;
        private GamePlayHud _hud;

        public override void Initialize()
        {
            base.Initialize();
            var container = ScreenManager.Container;

            _systems = container.GetServices<IGamePlaySystem>().OrderByDescending(x => x.Priority).ToList();
            _world = container.GetService<IWorld>();
            _viewport = container.GetService<IViewport>();
            _publisher = container.GetService<IEventPublisher>();
            _musicPlayer = container.GetService<IMusicPlayer>();

            var factory = container.GetService<IEntityFactory>();
            var ship = factory.CreateShip(new Vector2(_viewport.Width / 2.0f, _viewport.Height / 2.0f));
            var content = container.GetService<IContentProvider>();
            var context = container.GetService<GamePlayContext>();
            _controller = new PlayerShipKeyboardAndGamePadController(ship);

            var timer3 = new Timer(TimeSpan.FromSeconds(35), GameTags.NextHasardSituation, _publisher);

            _hud = new GamePlayHud
            (
                container.GetService<IOptionsMonitor<GameSettings>>(),
                container.GetService<IViewport>(),
                container.GetService<IPainter>(),
                container.GetService<IContentProvider>(),
                container.GetService<IFontService>(),
                context
            );

            _world.Add
            (
                ship,
                timer3
            );
            
            var file =
                content
                    .GetFiles("Music")
                    .Where(file => file.Contains("game"))
                    .RandomPick();

            context.Initialize();
            _musicPlayer.Stop();
            _musicPlayer.Play(content.Load<Music>(file));
        }

        public override void Free()
        {
            _musicPlayer.Stop();
            _world.Free();

            base.Free();
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            if (input.IsNewKeyPress(Keys.Escape, null, out _) || input.IsNewButtonPress(Buttons.Start, null, out _))
            {
                _musicPlayer.Pause();
                const string message = "Exit game?\nA button, Space, Enter = ok\nB button, Esc = cancel";
                var confirmExitMessageBox = new MessageBoxScreen(message);

                confirmExitMessageBox.Accepted += (_, __) => LoadingScreen.Load(ScreenManager, false, null, new StarScreen(), new MainMenuScreen());
                confirmExitMessageBox.Cancelled += (_, __) => _musicPlayer.Resume();

                ScreenManager.AddScreen(confirmExitMessageBox, null);
            }

            _controller.Handle(input);
        }

        public override void Update(XTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (!otherScreenHasFocus)
            {
                var time = (float)gameTime.ElapsedGameTime.TotalSeconds;

                _systems.Iter(system => system.Update(time));
            }
        }

        public override void Draw(XTime gameTime)
        {
            base.Draw(gameTime);

            var time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _world.Where(x => x is IDrawable).Cast<IDrawable>().Iter(x => x.Draw(time));
            _hud.Draw(time);
        }
    }
}
