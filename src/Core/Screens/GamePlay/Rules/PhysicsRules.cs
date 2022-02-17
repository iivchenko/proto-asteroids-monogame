using Core.Entities;
using Core.Leaderboards;
using Engine.Entities;
using Engine.Rules;
using Engine.Screens;
using Engine;
using Engine.Collisions;
using System;
using System.Linq;
using System.Numerics;

namespace Core.Screens.GamePlay.Rules
{
    public static class PhysicsRules
    {
        public abstract class WhenAsteroidCollidesPlayerShip : IRule<BodiesCollideEvent>
        {
            protected WhenAsteroidCollidesPlayerShip(GamePlayContext context)
            {
                Context = context;
            }

            protected GamePlayContext Context { get; }

            public bool ExecuteCondition(BodiesCollideEvent @event)
            {
                return (@event.Body1, @event.Body2)
                switch
                {
                    (Ship ship, Asteroid asteroid) => ExecuteConditionInternal(ship, asteroid),
                    (Asteroid asteroid, Ship ship) => ExecuteConditionInternal(ship, asteroid),
                    _ => false
                };
            }

            public void ExecuteAction(BodiesCollideEvent @event)
            {
                switch (@event.Body1, @event.Body2)
                {
                    case (Ship ship, Asteroid asteroid):
                        ExecuteActionInternal(ship, asteroid);
                        break;

                    case (Asteroid asteroid, Ship ship):
                        ExecuteActionInternal(ship, asteroid);
                        break;
                };
            }

            protected virtual bool ExecuteConditionInternal(Ship ship, Asteroid asteroid) => ship.State == ShipState.Alive && asteroid.State == AsteroidState.Alive;

            protected abstract void ExecuteActionInternal(Ship ship, Asteroid asteroid);

            public sealed class ThenDestroyAsteroid : WhenAsteroidCollidesPlayerShip
            {
                public ThenDestroyAsteroid(GamePlayContext context) 
                    : base(context)
                {
                }

                protected override void ExecuteActionInternal(Ship ship, Asteroid asteroid) => asteroid.Destroy();
            }

            public abstract class AndPlayerShipHasEnoughLifes : WhenAsteroidCollidesPlayerShip
            {
                protected AndPlayerShipHasEnoughLifes(GamePlayContext context)
                    : base(context)
                {
                }

                protected override bool ExecuteConditionInternal(Ship ship, Asteroid asteroid)
                    => base.ExecuteConditionInternal(ship, asteroid) && Context.Lifes > 0;

                public sealed class ThenReduceLifes : AndPlayerShipHasEnoughLifes
                {
                    public ThenReduceLifes(GamePlayContext context)
                        : base(context)
                    {
                    }

                    protected override void ExecuteActionInternal(Ship ship, Asteroid asteroid) => Context.Lifes--;
                }

                public sealed class ThenDestroyPlayersShip : AndPlayerShipHasEnoughLifes
                {
                    public ThenDestroyPlayersShip(GamePlayContext context)
                        : base(context)
                    {
                    }

                    protected override void ExecuteActionInternal(Ship ship, Asteroid asteroid) => ship.Destroy();
                }
            }

            public abstract class OrPlayerShipDoesntHaveEnoughLifes : WhenAsteroidCollidesPlayerShip
            {
                protected OrPlayerShipDoesntHaveEnoughLifes(GamePlayContext context) 
                    : base(context)
                {
                }

                protected override bool ExecuteConditionInternal(Ship ship, Asteroid asteroid)
                   => base.ExecuteConditionInternal(ship, asteroid) && Context.Lifes <= 0;

                public sealed class ThenRemovePlayersShipFromTheGame : OrPlayerShipDoesntHaveEnoughLifes
                {
                    private readonly IWorld _world;

                    public ThenRemovePlayersShipFromTheGame(GamePlayContext context, IWorld world)
                        : base(context)
                    {
                        _world = world;
                    }

                    protected override void ExecuteActionInternal(Ship ship, Asteroid asteroid) => _world.Remove(ship);
                }

                public sealed class ThenGameOver : OrPlayerShipDoesntHaveEnoughLifes
                {
                    private readonly LeaderboardsManager _leaderBoard;

                    public ThenGameOver(
                        GamePlayContext context,
                        LeaderboardsManager leaderBoard)
                        : base(context)
                    {
                        _leaderBoard = leaderBoard;
                    }

                    protected override void ExecuteActionInternal(Ship ship, Asteroid asteroid)
                    {
                        var playedTime = DateTime.Now - Context.StartTime;

                        if (_leaderBoard.CanAddLeader(Context.Scores))
                        {
                            var newHigthScorePrompt = new PromptScreen("Congratulations, you made new high score!\nEnter you name:");

                            newHigthScorePrompt.Accepted += (_, __) =>
                            {
                                _leaderBoard.AddLeader(newHigthScorePrompt.Text, Context.Scores, playedTime);
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

        public abstract class WhenPlayersProjectileCollidesAsteroid : IRule<BodiesCollideEvent>
        {
            public bool ExecuteCondition(BodiesCollideEvent @event)
            {
                    return (@event.Body1, @event.Body2)
                    switch 
                    {
                        (Projectile projectile, Asteroid asteroid) => ExecuteConditionInternal(projectile, asteroid),
                        (Asteroid asteroid, Projectile projectile) => ExecuteConditionInternal(projectile, asteroid),
                        _ => false
                    };
            }

            public void ExecuteAction(BodiesCollideEvent @event)
            {                
                switch (@event.Body1, @event.Body2)
                {
                    case (Projectile projectile, Asteroid asteroid) :
                        ExecuteActionInternal(projectile, asteroid);
                        break;

                    case (Asteroid asteroid, Projectile projectile) :
                        ExecuteActionInternal(projectile, asteroid);
                        break;
                };
            }

            protected abstract bool ExecuteConditionInternal(Projectile projectile, Asteroid asteroid);

            protected abstract void ExecuteActionInternal(Projectile projectile, Asteroid asteroid);

            public abstract class AndAsteroidIsAlive : WhenPlayersProjectileCollidesAsteroid
            {
                protected override bool ExecuteConditionInternal(Projectile projectile, Asteroid asteroid) => asteroid.State == AsteroidState.Alive;

                public sealed class ThenScore: AndAsteroidIsAlive
                {
                    private readonly GamePlayContext _context;
                    private readonly GamePlayScoreManager _scores;

                    public ThenScore(GamePlayContext context)
                    {
                        _context = context;
                        _scores = new GamePlayScoreManager();
                    }

                    protected override void ExecuteActionInternal(Projectile projectile, Asteroid asteroid) => _context.Scores += _scores.GetScore(asteroid);
                }

                public sealed class ThenRemoveProjectile : AndAsteroidIsAlive
                {
                    private readonly IWorld _world;

                    public ThenRemoveProjectile(
                       IWorld world)
                    {
                        _world = world;
                    }

                    protected override void ExecuteActionInternal(Projectile projectile, Asteroid asteroid) => _world.Remove(projectile);
                }

                public sealed class ThenDestroyAsteroid : AndAsteroidIsAlive
                {
                    protected override void ExecuteActionInternal(Projectile projectile, Asteroid asteroid) => asteroid.Destroy();
                }

                public abstract class AndAsteroidIsBig : AndAsteroidIsAlive
                {
                    protected override bool ExecuteConditionInternal(Projectile projectile, Asteroid asteroid) =>
                        base.ExecuteConditionInternal(projectile, asteroid) && asteroid.Type == AsteroidType.Big;

                    public sealed class TheFallAsteroidAppart : AndAsteroidIsBig
                    {
                        private readonly IWorld _world;
                        private readonly IEntityFactory _entityFactory;

                        public TheFallAsteroidAppart(
                           IWorld world,
                           IEntityFactory entityFactory)
                        {
                            _world = world;
                            _entityFactory = entityFactory;
                        }

                        protected override void ExecuteActionInternal(Projectile projectile, Asteroid asteroid)
                        {
                            var offset = new Vector2(23);
                            var direction1 = asteroid.Velocity.ToRotation() - 30.AsRadians();
                            var direction2 = asteroid.Velocity.ToRotation() + 30.AsRadians();
                            var position1 = asteroid.Position - offset;
                            var position2 = asteroid.Position + offset;
                            var med1 = _entityFactory.CreateAsteroid(AsteroidType.Medium, position1, direction1);
                            var med2 = _entityFactory.CreateAsteroid(AsteroidType.Medium, position2, direction2);

                            _world.Add(med1, med2);
                        }
                    }
                }
            }
        }

        public abstract class WhenAsteroidCollidesAsteroid : IRule<BodiesCollideEvent>
        {
            public bool ExecuteCondition(BodiesCollideEvent @event)
            {
                return (@event.Body1, @event.Body2)
                switch
                {
                    (Asteroid asteroid1, Asteroid asteroid2) 
                        when asteroid1.State == AsteroidState.Alive && asteroid2.State == AsteroidState.Alive 
                            => ExecuteConditionInternal(asteroid1, asteroid2),
                    _ => false
                };
            }

            public void ExecuteAction(BodiesCollideEvent @event)
            {
                switch (@event.Body1, @event.Body2)
                {
                    case (Asteroid asteroid1, Asteroid asteroid2):
                        ExecuteActionInternal(asteroid1, asteroid2);
                        break;
                };
            }

            protected abstract bool ExecuteConditionInternal(Asteroid asteroid1, Asteroid asteroid2);

            protected abstract void ExecuteActionInternal(Asteroid asteroid1, Asteroid asteroid2);

            public sealed class ThenDestroyAsteroids : WhenAsteroidCollidesAsteroid
            {
                protected override bool ExecuteConditionInternal(Asteroid asteroid1, Asteroid asteroid2) => true;

                protected override void ExecuteActionInternal(Asteroid asteroid1, Asteroid asteroid2)
                {
                    asteroid1.Destroy();
                    asteroid2.Destroy();
                }
            }

            public abstract class AndAsteroidIsBig : WhenAsteroidCollidesAsteroid
            {
                protected override bool ExecuteConditionInternal(Asteroid asteroid1, Asteroid asteroid2) =>
                    asteroid1.Type == AsteroidType.Big || asteroid2.Type == AsteroidType.Big;

                public sealed class TheFallAsteroidAppart : AndAsteroidIsBig
                {
                    private readonly IWorld _world;
                    private readonly IEntityFactory _entityFactory;

                    public TheFallAsteroidAppart(
                       IWorld world,
                       IEntityFactory entityFactory)
                    {
                        _world = world;
                        _entityFactory = entityFactory;
                    }

                    protected override void ExecuteActionInternal(Asteroid asteroid1, Asteroid asteroid2)
                    {
                        var offset = new Vector2(23);
                        if (asteroid1.Type == AsteroidType.Big)
                        {
                            var direction1 = asteroid1.Velocity.ToRotation() - 30.AsRadians();
                            var direction2 = asteroid1.Velocity.ToRotation() + 30.AsRadians();
                            var position1 = asteroid1.Position - offset;
                            var position2 = asteroid1.Position + offset;
                            var med1 = _entityFactory.CreateAsteroid(AsteroidType.Medium, position1, direction1);
                            var med2 = _entityFactory.CreateAsteroid(AsteroidType.Medium, position2, direction2);

                            _world.Add(med1, med2);
                        }

                        if (asteroid2.Type == AsteroidType.Big)
                        {
                            var direction1 = asteroid2.Velocity.ToRotation() - 30.AsRadians();
                            var direction2 = asteroid2.Velocity.ToRotation() + 30.AsRadians();
                            var position1 = asteroid1.Position - offset;
                            var position2 = asteroid1.Position + offset;
                            var med1 = _entityFactory.CreateAsteroid(AsteroidType.Medium, position1, direction1);
                            var med2 = _entityFactory.CreateAsteroid(AsteroidType.Medium, position2, direction2);

                            _world.Add(med1, med2);
                        }
                    }
                }
            }
        }
    }
}
