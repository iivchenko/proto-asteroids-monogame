using KenneyAsteroids.Core.Entities;
using KenneyAsteroids.Core.Leaderboards;
using KenneyAsteroids.Engine.Entities;
using KenneyAsteroids.Engine.Graphics;
using KenneyAsteroids.Engine.Rules;
using KenneyAsteroids.Engine.Screens;
using System;
using System.Numerics;
using KenneyAsteroids.Engine;
using System.Linq;

namespace KenneyAsteroids.Core.Screens.GamePlay.Rules
{
    public static class PhysicsRules
    {
        public static class WhenAsteroidCollidesPlayerShip
        {
            public static class AndPlayerShipHasEnoughLifes
            {
                public sealed class ThenReduceLifes : IRule<GamePlayEntitiesCollideEvent<Ship, Asteroid>>
                {
                    private readonly IEntitySystem _entities;

                    public ThenReduceLifes(IEntitySystem entities)
                    {
                        _entities = entities;
                    }

                    public bool ExecuteCondition(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event) 
                        => @event.Body1.State == ShipState.Alive && @event.Body2.State == AsteroidState.Alive && GetHud().Lifes > 0;

                    public void ExecuteAction(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event) => GetHud().Lifes--;

                    private GamePlayHud GetHud() => _entities.First(x => x is GamePlayHud) as GamePlayHud;
                }

                public sealed class ThenDestroyPlayersShip : IRule<GamePlayEntitiesCollideEvent<Ship, Asteroid>>
                {
                    private readonly IEntitySystem _entities;

                    public ThenDestroyPlayersShip(IEntitySystem entities)
                    {
                        _entities = entities;
                    }

                    public bool ExecuteCondition(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event)
                        => @event.Body1.State == ShipState.Alive && @event.Body2.State == AsteroidState.Alive && GetHud().Lifes > 0;

                    public void ExecuteAction(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event) => @event.Body1.Destroy();

                    private GamePlayHud GetHud()
                        => _entities.First(x => x is GamePlayHud) as GamePlayHud;
                }

                public sealed class ThenDestroyAsteroid : IRule<GamePlayEntitiesCollideEvent<Ship, Asteroid>>
                {
                    public bool ExecuteCondition(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event)
                        => @event.Body1.State == ShipState.Alive && @event.Body2.State == AsteroidState.Alive;

                    public void ExecuteAction(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event) => @event.Body2.Destroy();
                }
            }

            public static class AndPlayerShipDoesntHaveEnoughLifes
            {
                public sealed class ThenRemovePlayersShipFromTheGame : IRule<GamePlayEntitiesCollideEvent<Ship, Asteroid>>
                {
                    private readonly IEntitySystem _entities;

                    public ThenRemovePlayersShipFromTheGame(IEntitySystem entities)
                    {
                        _entities = entities;
                    }

                    public bool ExecuteCondition(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event)
                        => @event.Body1.State == ShipState.Alive && @event.Body2.State == AsteroidState.Alive && GetHud().Lifes <= 0;

                    public void ExecuteAction(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event) => _entities.Remove(@event.Body1);

                    private GamePlayHud GetHud() => _entities.First(x => x is GamePlayHud) as GamePlayHud;

                }

                public sealed class ThenGameOver : IRule<GamePlayEntitiesCollideEvent<Ship, Asteroid>>
                {
                    private readonly IEntitySystem _entities;
                    private readonly LeaderboardsManager _leaderBoard;

                    public ThenGameOver(
                        IEntitySystem entities,
                        LeaderboardsManager leaderBoard)
                    {
                        _entities = entities;
                        _leaderBoard = leaderBoard;
                    }

                    public bool ExecuteCondition(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event) => GetHud().Lifes <= 0;

                    public void ExecuteAction(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event)
                    {
                        var ship = @event.Body1;

                        var playedTime = DateTime.Now - GetHud().StartTime;

                        if (_leaderBoard.CanAddLeader(GetHud().Scores))
                        {
                            var newHigthScorePrompt = new PromptScreen("Congratulations, you made new high score!\nEnter you name:");

                            newHigthScorePrompt.Accepted += (_, __) =>
                            {
                                _leaderBoard.AddLeader(newHigthScorePrompt.Text, GetHud().Scores, playedTime);
                                GameOverMessage();
                            };
                            newHigthScorePrompt.Cancelled += (_, __) => GameOverMessage();

                            GameRoot.ScreenManager.AddScreen(newHigthScorePrompt, null);
                        }
                        else
                        {
                            GameOverMessage();
                        }
                    }

                    private void GameOverMessage()
                    {
                        const string message = "GAME OVER?\nA button, Space, Enter = Restart\nB button, Esc = Exit";
                        var msg = new MessageBoxScreen(message);

                        msg.Accepted += (_, __) => LoadingScreen.Load(GameRoot.ScreenManager, false, null, new StarScreen(), new GamePlayScreen());
                        msg.Cancelled += (_, __) => LoadingScreen.Load(GameRoot.ScreenManager, false, null, new StarScreen(), new MainMenuScreen());

                        GameRoot.ScreenManager.AddScreen(msg, null);
                    }

                    private GamePlayHud GetHud() => _entities.First(x => x is GamePlayHud) as GamePlayHud;
                }
            }
        }

        public static class WhenPlayersProjectileCollidesAsteroid
        {
            public static class AndAsteroidIsAlive
            {
                public sealed class ThenScore : IRule<GamePlayEntitiesCollideEvent<Projectile, Asteroid>>
                {
                    private readonly IEntitySystem _entities;
                    private readonly GamePlayScoreManager _scores;

                    public ThenScore(IEntitySystem entities)
                    {
                        _entities = entities;
                        _scores = new GamePlayScoreManager();
                    }

                    public bool ExecuteCondition(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event) => @event.Body2.State == AsteroidState.Alive;

                    public void ExecuteAction(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event)
                    {
                        GetHud().Scores += _scores.GetScore(@event.Body2);
                    }

                    private GamePlayHud GetHud() => _entities.First(x => x is GamePlayHud) as GamePlayHud;

                }

                public sealed class ThenRemoveProjectile : IRule<GamePlayEntitiesCollideEvent<Projectile, Asteroid>>
                {
                    private readonly IEntitySystem _entities;

                    public ThenRemoveProjectile(
                       IEntitySystem entities)
                    {
                        _entities = entities;
                    }

                    public bool ExecuteCondition(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event) => @event.Body2.State == AsteroidState.Alive;

                    public void ExecuteAction(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event)
                    {
                        _entities.Remove(@event.Body1);
                    }
                }

                public sealed class ThenDestroyAsteroid : IRule<GamePlayEntitiesCollideEvent<Projectile, Asteroid>>
                {
                    public bool ExecuteCondition(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event) => @event.Body2.State == AsteroidState.Alive;

                    public void ExecuteAction(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event)
                    {
                        @event.Body2.Destroy();
                    }
                }

                public static class AndAsteroidIsBig
                {
                    public sealed class TheFallAsteroidAppart : IRule<GamePlayEntitiesCollideEvent<Projectile, Asteroid>>
                    {
                        private readonly IEntitySystem _entities;
                        private readonly IEntityFactory _entityFactory;

                        public TheFallAsteroidAppart(
                           IEntitySystem entities,
                           IEntityFactory entityFactory)
                        {
                            _entities = entities;
                            _entityFactory = entityFactory;
                        }

                        public bool ExecuteCondition(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event) => @event.Body2.State == AsteroidState.Alive && @event.Body2.Type == AsteroidType.Big;

                        public void ExecuteAction(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event)
                        {
                            var asteroid = @event.Body2;
                            var direction1 = asteroid.Velocity.ToRotation() - 20.AsRadians();
                            var direction2 = asteroid.Velocity.ToRotation() + 20.AsRadians();
                            var position1 = asteroid.Position;
                            var position2 = asteroid.Position;
                            var med1 = _entityFactory.CreateAsteroid(AsteroidType.Medium, position1, direction1);
                            var med2 = _entityFactory.CreateAsteroid(AsteroidType.Medium, position2, direction2);

                            _entities.Add(med1, med2);
                        }
                    }
                }
            }
        }
    }
}
