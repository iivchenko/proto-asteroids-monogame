using Core.Entities;
using Engine.Entities;
using Engine.Screens;
using Engine.Collisions;
using System;

namespace Core.Screens.GamePlay.Events
{
    public sealed class AsteroidAndPlayerShipCollisionEventHandler : BaseCollisionEventHandler<Asteroid, Ship>
    {
        private readonly GamePlayContext _context;
        private readonly IWorld _world;
        private readonly ICollisionService _collisionService;

        public AsteroidAndPlayerShipCollisionEventHandler(
            GamePlayContext context,
            IWorld world,
            ICollisionService collisionService)
        {
            _context = context;
            _world = world;
            _collisionService = collisionService;
        }

        protected override bool ExecuteConditionInternal(Asteroid asteroid, Ship ship)
           => ship.State == ShipState.Alive && asteroid.State == AsteroidState.Alive;

        protected override void ExecuteActionInternal(Asteroid asteroid, Ship ship)
        {
            _context.Lifes--;
            asteroid.Destroy();

            if (_context.Lifes > 0)
            {
                ship.Destroy();
            }
            else
            {
                _world.Remove(ship);
                _collisionService.UnregisterBody(ship);

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
