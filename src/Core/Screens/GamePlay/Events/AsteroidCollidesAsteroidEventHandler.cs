using Core.Entities;
using Engine.Entities;
using Engine;
using System.Numerics;

namespace Core.Screens.GamePlay.Events
{
    public sealed class AsteroidCollidesAsteroidEventHandler : BaseCollisionEventHandler<Asteroid, Asteroid>
    {
        private readonly IWorld _world;
        private readonly IEntityFactory _entityFactory;

        public AsteroidCollidesAsteroidEventHandler(
           IWorld world,
           IEntityFactory entityFactory)
        {
            _world = world;
            _entityFactory = entityFactory;
        }

        protected override bool ExecuteConditionInternal(Asteroid asteroid1, Asteroid asteroid2)
            => asteroid1.State == AsteroidState.Alive && asteroid2.State == AsteroidState.Alive;

        protected override void ExecuteActionInternal(Asteroid asteroid1, Asteroid asteroid2)
        {
            asteroid1.Destroy();
            asteroid2.Destroy();

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
