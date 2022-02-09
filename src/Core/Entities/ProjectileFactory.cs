using Engine;
using Engine.Content;
using Engine.Graphics;
using System.Numerics;

namespace Core.Entities
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
            const float Scale = 4.0f;
            var rotation = direction.ToRotation();

            return new Projectile(_draw, sprite, rotation, Speed)
            {
                Position = position,
                Scale = new Vector2(Scale)
            };
        }
    }
}
