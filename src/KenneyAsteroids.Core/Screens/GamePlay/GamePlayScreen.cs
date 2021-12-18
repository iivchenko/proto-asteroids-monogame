using KenneyAsteroids.Core.Entities;
using KenneyAsteroids.Engine;
using KenneyAsteroids.Engine.Audio;
using KenneyAsteroids.Engine.Collisions;
using KenneyAsteroids.Engine.Content;
using KenneyAsteroids.Engine.Entities;
using KenneyAsteroids.Engine.Graphics;
using KenneyAsteroids.Engine.Rules;
using KenneyAsteroids.Engine.Screens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Numerics;

using XTime = Microsoft.Xna.Framework.GameTime;
using System;
using System.Collections.Generic;

namespace KenneyAsteroids.Core.Screens.GamePlay
{
    public sealed class GamePlayScreen : GameScreen
    {
        private List<IGamePlaySystem> _systems;
        private IEntitySystem _entities;
        private ICollisionSystem _collisions;
        private IViewport _viewport;
        private IEventPublisher _publisher;
        private IMusicPlayer _musicPlayer;

        private ShipPlayerController _controller;

        public override void Initialize()
        {
            base.Initialize();
            var container = ScreenManager.Container;

            _systems = container.GetServices<IGamePlaySystem>().OrderByDescending(x => x.Priority).ToList();
            _entities = container.GetService<IEntitySystem>();
            _viewport = container.GetService<IViewport>();
            _publisher = container.GetService<IEventPublisher>();
            _musicPlayer = container.GetService<IMusicPlayer>();
            _collisions = new CollisionSystem();

            var factory = container.GetService<IEntityFactory>();
            var ship = factory.CreateShip(new Vector2(_viewport.Width / 2.0f, _viewport.Height / 2.0f));
            var content = container.GetService<IContentProvider>();

            _controller = new ShipPlayerController(ship);

            var timer1 = new Timer(TimeSpan.FromSeconds(3), GameTags.NextAsteroid, _publisher);
            var timer2 = new Timer(TimeSpan.FromSeconds(60), GameTags.NextAsteroidLimitChange, _publisher);
            var timer3 = new Timer(TimeSpan.FromSeconds(45), GameTags.NextHasardSituation, _publisher);
            var hud = new GamePlayHud
            (
                container.GetService<IOptionsMonitor<GameSettings>>(),
                container.GetService<IViewport>(),
                container.GetService<IPainter>(),
                container.GetService<IContentProvider>(),
                container.GetService<IFontService>()
            );

            _entities.Add
            (
                ship,
                hud,
                timer1,
                timer2,
                timer3
            );
            
            var file =
                content
                    .GetFiles("Music")
                    .Where(file => file.Contains("game"))
                    .RandomPick();

            _musicPlayer.Play(content.Load<Music>(file));
        }

        public override void Free()
        {
            _musicPlayer.Stop();
            _entities.Remove(_entities.ToArray());
            _entities.Commit();

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

            var time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!otherScreenHasFocus)
            {
                _entities
                    .Where(x => x is IUpdatable)
                    .Cast<IUpdatable>()
                    .Iter(x => x.Update(time));

                var bodies = _entities.Where(x => x is IBody).Cast<IBody>();

                // Move Collision eventing into collision system
                foreach (var collision in _collisions.EvaluateCollisions(bodies))
                {
                    switch ((collision.Body1, collision.Body2))
                    {
                        case (Ship ship, Asteroid asteroid):
                            _publisher.Publish(new GamePlayEntitiesCollideEvent<Ship, Asteroid>(ship, asteroid));
                            break;

                        case (Asteroid asteroid, Ship ship):
                            _publisher.Publish(new GamePlayEntitiesCollideEvent<Ship, Asteroid>(ship, asteroid));
                            break;

                        case (Projectile projectile, Asteroid asteroid):
                            _publisher.Publish(new GamePlayEntitiesCollideEvent<Projectile, Asteroid>(projectile, asteroid));
                            break;

                        case (Asteroid asteroid, Projectile projectile):
                            _publisher.Publish(new GamePlayEntitiesCollideEvent<Projectile, Asteroid>(projectile, asteroid));
                            break;
                    }
                }

                _systems.Iter(system => system.Update(time));

                _entities.Commit();
            }
        }

        public override void Draw(XTime gameTime)
        {
            base.Draw(gameTime);

            var time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _entities.Where(x => x is IDrawable).Cast<IDrawable>().Iter(x => x.Draw(time));
        }
    }
}
