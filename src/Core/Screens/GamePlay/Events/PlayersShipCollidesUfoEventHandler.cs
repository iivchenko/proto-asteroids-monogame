using Core.Entities;
using Engine.Entities;
using Engine.Screens;
using Engine.Collisions;
using System;

namespace Core.Screens.GamePlay.Events
{
    public sealed class PlayersShipCollidesUfoEventHandler : BaseCollisionEventHandler<Ship, Ufo>
    {
        private readonly GamePlayContext _context;
        private readonly IWorld _world;
        private readonly ICollisionService _collisionService;

        public PlayersShipCollidesUfoEventHandler(
            GamePlayContext context,
            IWorld world,
            ICollisionService collisionService)
        {
            _context = context;
            _world = world;
            _collisionService = collisionService;
        }

        protected override bool ExecuteConditionInternal(Ship ship, Ufo ufo)
            => ship.State == ShipState.Alive && ufo.State == UfoState.Alive;

        protected override void ExecuteActionInternal(Ship ship, Ufo _)
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