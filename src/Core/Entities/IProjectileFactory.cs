using Engine.Graphics;
using System.Numerics;

namespace Core.Entities
{
    public interface IProjectileFactory
    {
        Projectile Create(Vector2 position, Vector2 direction, Sprite sprite);
    }
}
