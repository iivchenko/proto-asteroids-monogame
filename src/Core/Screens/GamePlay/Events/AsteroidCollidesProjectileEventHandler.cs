using Core.Entities;
using Engine.Entities;
using Engine;
using Engine.Collisions;
using System;
using System.Linq;
using System.Numerics;

namespace Core.Screens.GamePlay.Events
{
    public sealed class AsteroidCollidesProjectileEventHandler : BaseCollisionEventHandler<Asteroid, Projectile>
    {
        private readonly GamePlayContext _context;
        private readonly GamePlayScoreManager _scores;
        private readonly IWorld _world;
        private readonly ICollisionService _collisionService;
        private readonly IEntityFactory _entityFactory;

        public AsteroidCollidesProjectileEventHandler(
            GamePlayContext context,
            GamePlayScoreManager scores,
            IWorld world,
            ICollisionService collisionService,
            IEntityFactory entityFactory)
        {
            _context = context;
            _scores = scores;
            _world = world;
            _collisionService = collisionService;
            _entityFactory = entityFactory;
        }

        protected override bool ExecuteConditionInternal(Asteroid asteroid, Projectile projectile)
            => asteroid.State == AsteroidState.Alive;

        protected override void ExecuteActionInternal(Asteroid asteroid, Projectile projectile)
        {
            _world.Remove(projectile);
            _collisionService.UnregisterBody(projectile);

            asteroid.Destroy();

            if (asteroid.Type == AsteroidType.Big)
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

            if (projectile.Tags.Contains(GameTags.Player))
            {
                _context.Scores += _scores.GetScore(asteroid);
            }
        }
    }
}
