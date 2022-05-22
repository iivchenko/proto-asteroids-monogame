using Core.Entities;
using Engine;
using Engine.Collisions;
using Engine.Entities;
using Engine.Graphics;
using System.Linq;
using System.Numerics;

namespace Core.Screens.GamePlay.Systems
{
    public sealed class OutOfScreenSystem : IGamePlaySystem
    {
        private readonly IWorld _world;
        private readonly ICollisionService _collisionService;
        private readonly IViewport _viewport;

        public OutOfScreenSystem(IWorld world, ICollisionService collisionService, IViewport viewport)
        {
            _world = world;
            _collisionService = collisionService;
            _viewport = viewport;
        }

        public uint Priority => 10;

        public void Update(float time)
        {
            _world
                .Where(x => x is IBody).Cast<IBody>()
                .Where(IsOutOfScreen)
                .Iter(HandleOutOfScreenBodies);
        }

        private bool IsOutOfScreen(IBody entity)
        {
            return
                entity.Position.X + entity.Width / 2.0 < 0 ||
                entity.Position.X - entity.Width / 2.0 > _viewport.Width ||
                entity.Position.Y + entity.Height / 2.0 < 0 ||
                entity.Position.Y - entity.Height / 2.0 > _viewport.Height;
        }

        private void HandleOutOfScreenBodies(IBody body)
        {
            switch (body)
            {
                case Projectile projectile:
                    _world.Remove(projectile);
                    _collisionService.UnregisterBody(projectile);
                    break;
                default:
                    var x = body.Position.X;
                    var y = body.Position.Y;

                    if (body.Position.X + body.Width / 2.0f < 0)
                    {
                        x = _viewport.Width + body.Width / 2.0f;
                    }
                    else if (body.Position.X - body.Width / 2.0f > _viewport.Width)
                    {
                        x = 0 - body.Width / 2.0f;
                    }

                    if (body.Position.Y + body.Height / 2.0f < 0)
                    {
                        y = _viewport.Height + body.Height / 2.0f;
                    }
                    else if (body.Position.Y - body.Height / 2.0f > _viewport.Height)
                    {
                        y = 0 - body.Height / 2.0f;
                    }

                    body.Position = new Vector2(x, y);
                    break;
            }
        }
    }
}
