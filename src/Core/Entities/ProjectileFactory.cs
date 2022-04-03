using Engine;
using Engine.Collisions;
using Engine.Graphics;
using System.Numerics;

namespace Core.Entities
{
    public sealed class ProjectileFactory : IProjectileFactory
    {
        private readonly IPainter _draw;
        private readonly ICollisionService _collisionSystem;

        public ProjectileFactory(
            IPainter draw,
            ICollisionService collisionSystem)
        {
            _draw = draw;
            _collisionSystem = collisionSystem;
        }

        public Projectile Create(Vector2 position, Vector2 direction, Sprite sprite, string tag)
        {
            const float Speed = 1200.0f;
            var rotation = direction.ToRotation();

            var projectile = new Projectile(_draw, sprite, rotation, Speed)
            {
                Position = position,
                Scale = new Vector2(GameRoot.Scale),
                Tags = new[] { tag }
            };

            _collisionSystem.RegisterBody(projectile, sprite);

            return projectile;
        }
    }
}
