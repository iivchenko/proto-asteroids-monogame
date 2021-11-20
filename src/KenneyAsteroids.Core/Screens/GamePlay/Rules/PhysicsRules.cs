using KenneyAsteroids.Core.Entities;
using KenneyAsteroids.Core.Leaderboards;
using KenneyAsteroids.Engine.Entities;
using KenneyAsteroids.Engine.Rules;
using KenneyAsteroids.Engine.Screens;
using System;
using KenneyAsteroids.Engine;
using System.Linq;

namespace KenneyAsteroids.Core.Screens.GamePlay.Rules
{
    public static class PhysicsRules
    {
        public abstract class WhenAsteroidCollidesPlayerShip : IRule<GamePlayEntitiesCollideEvent<Ship, Asteroid>>
        {
            protected WhenAsteroidCollidesPlayerShip(IEntitySystem entities)
            {
                Entities = entities;
            }

            protected IEntitySystem Entities { get; }

            protected GamePlayHud GetHud() => Entities.First(x => x is GamePlayHud) as GamePlayHud;

            public virtual bool ExecuteCondition(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event)
                => @event.Body1.State == ShipState.Alive && @event.Body2.State == AsteroidState.Alive;

            public abstract void ExecuteAction(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event);

            public sealed class ThenDestroyAsteroid : WhenAsteroidCollidesPlayerShip
            {
                public ThenDestroyAsteroid(IEntitySystem entities) 
                    : base(entities)
                {
                }

                public override void ExecuteAction(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event) => @event.Body2.Destroy();
            }

            public abstract class AndPlayerShipHasEnoughLifes : WhenAsteroidCollidesPlayerShip
            {
                protected AndPlayerShipHasEnoughLifes(IEntitySystem entities)
                    : base(entities)
                {
                }

                public override bool ExecuteCondition(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event)
                    => base.ExecuteCondition(@event) && GetHud().Lifes > 0;

                public sealed class ThenReduceLifes : AndPlayerShipHasEnoughLifes
                {
                    public ThenReduceLifes(IEntitySystem entities)
                        : base (entities)
                    {
                    }

                    public override void ExecuteAction(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event) => GetHud().Lifes--;
                }

                public sealed class ThenDestroyPlayersShip : AndPlayerShipHasEnoughLifes
                {
                    public ThenDestroyPlayersShip(IEntitySystem entities)
                        : base(entities)
                    {
                    }

                    public override void ExecuteAction(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event) => @event.Body1.Destroy();
                }
            }

            public abstract class OrPlayerShipDoesntHaveEnoughLifes : WhenAsteroidCollidesPlayerShip
            {
                protected OrPlayerShipDoesntHaveEnoughLifes(IEntitySystem entities) 
                    : base(entities)
                {
                }

                public override bool ExecuteCondition(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event)
                   => base.ExecuteCondition(@event) && GetHud().Lifes <= 0;

                public sealed class ThenRemovePlayersShipFromTheGame : OrPlayerShipDoesntHaveEnoughLifes
                {
                    public ThenRemovePlayersShipFromTheGame(IEntitySystem entities)
                        : base(entities)
                    {
                    }

                    public override void ExecuteAction(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event) => Entities.Remove(@event.Body1);

                }

                public sealed class ThenGameOver : OrPlayerShipDoesntHaveEnoughLifes
                {
                    private readonly LeaderboardsManager _leaderBoard;

                    public ThenGameOver(
                        IEntitySystem entities,
                        LeaderboardsManager leaderBoard)
                        : base(entities)
                    {
                        _leaderBoard = leaderBoard;
                    }

                    public override void ExecuteAction(GamePlayEntitiesCollideEvent<Ship, Asteroid> @event)
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
                }
            }
        }

        public abstract class WhenPlayersProjectileCollidesAsteroid : IRule<GamePlayEntitiesCollideEvent<Projectile, Asteroid>>
        {
            public abstract bool ExecuteCondition(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event);

            public abstract void ExecuteAction(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event);

            public abstract class AndAsteroidIsAlive : WhenPlayersProjectileCollidesAsteroid
            {
                public override bool ExecuteCondition(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event) => @event.Body2.State == AsteroidState.Alive;

                public sealed class ThenScore: AndAsteroidIsAlive
                {
                    private readonly IEntitySystem _entities;
                    private readonly GamePlayScoreManager _scores;

                    public ThenScore(IEntitySystem entities)
                    {
                        _entities = entities;
                        _scores = new GamePlayScoreManager();
                    }

                    public override void ExecuteAction(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event)
                    {
                        GetHud().Scores += _scores.GetScore(@event.Body2);
                    }

                    private GamePlayHud GetHud() => _entities.First(x => x is GamePlayHud) as GamePlayHud;

                }

                public sealed class ThenRemoveProjectile : AndAsteroidIsAlive
                {
                    private readonly IEntitySystem _entities;

                    public ThenRemoveProjectile(
                       IEntitySystem entities)
                    {
                        _entities = entities;
                    }

                    public override void ExecuteAction(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event)
                    {
                        _entities.Remove(@event.Body1);
                    }
                }

                public sealed class ThenDestroyAsteroid : AndAsteroidIsAlive
                {
                    public override void ExecuteAction(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event)
                    {
                        @event.Body2.Destroy();
                    }
                }

                public abstract class AndAsteroidIsBig : AndAsteroidIsAlive
                {
                    public override bool ExecuteCondition(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event)
                        => base.ExecuteCondition(@event) && @event.Body2.Type == AsteroidType.Big;

                    public sealed class TheFallAsteroidAppart : AndAsteroidIsBig
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

                        public override void ExecuteAction(GamePlayEntitiesCollideEvent<Projectile, Asteroid> @event)
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
