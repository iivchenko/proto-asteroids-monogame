using KenneyAsteroids.Engine;
using KenneyAsteroids.Engine.Content;
using KenneyAsteroids.Engine.Graphics;
using System.Numerics;

namespace KenneyAsteroids.Core.Entities
{
    public sealed class ProjectileFactory : IProjectileFactory
    {
        private readonly IPainter _draw;

        public ProjectileFactory(
            IPainter draw)
        {
            _draw = draw;
        }

        public Projectile Create(Vector2 position, Vector2 direction, Sprite sprite)
        {
            const float Speed = 1200.0f;
            var rotation = direction.ToRotation();

            return new Projectile(_draw, sprite, rotation, Speed)
            {
                Position = position
            };
        }
    }
}
