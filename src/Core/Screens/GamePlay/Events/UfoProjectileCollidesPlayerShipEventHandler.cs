using Core.Entities;
using Engine.Entities;
using Engine.Screens;
using Engine.Collisions;
using System;
using System.Linq;

namespace Core.Screens.GamePlay.Events
{
    public sealed class UfoProjectileCollidesPlayerShipEventHandler : BaseCollisionEventHandler<Projectile, Ship>
    {
        private readonly GamePlayContext _context;
        private readonly IWorld _world;
        private readonly ICollisionService _collisionService;

        public UfoProjectileCollidesPlayerShipEventHandler(
            GamePlayContext context,
            IWorld world,
            ICollisionService collisionService)
        {
            _context = context;
            _world = world;
            _collisionService = collisionService;
        }

        protected override bool ExecuteConditionInternal(Projectile projectile, Ship ship)
            => projectile.Tags.Contains(GameTags.Enemy) && ship.State == ShipState.Alive;

        protected override void ExecuteActionInternal(Projectile projectile, Ship ship)
        {
            _context.Lifes--;

            if (_context.Lifes > 0)
            {
                ship.Destroy();
            }
            else
            {
                _world.Remove(ship);
                _collisionService.UnregisterBody(ship);

                var playedTime = DateTime.Now - _context.StartTime;

                GameOverMessage();
            }
        }

        private void GameOverMessage()
        {
            var message = $"GAME OVER?\n\nYour score is: {_context.Scores}\n\nA button, Space, Enter = Restart\nB button, Esc = Exit";
            var msg = new MessageBoxScreen(message);

            msg.Accepted += (_, __) => LoadingScreen.Load(GameRoot.ScreenManager, false, null, new StarScreen(), new GamePlayScreen());
            msg.Cancelled += (_, __) => LoadingScreen.Load(GameRoot.ScreenManager, false, null, new StarScreen(), new MainMenuScreen());

            GameRoot.ScreenManager.AddScreen(msg, null);
        }
    }
}
