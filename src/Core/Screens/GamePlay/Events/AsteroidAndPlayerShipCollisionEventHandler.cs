using Core.Entities;
using Core.Leaderboards;
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
        private readonly LeaderboardsManager _leaderBoard;

        public AsteroidAndPlayerShipCollisionEventHandler(
            GamePlayContext context,
            IWorld world,
            ICollisionService collisionService,
            LeaderboardsManager leaderBoard)
        {
            _context = context;
            _world = world;
            _collisionService = collisionService;
            _leaderBoard = leaderBoard;
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

                var playedTime = DateTime.Now - _context.StartTime;

                if (_leaderBoard.CanAddLeader(_context.Scores))
                {
                    var newHigthScorePrompt = new PromptScreen("Congratulations, you made new high score!\nEnter you name:");

                    newHigthScorePrompt.Accepted += (_, __) =>
                    {
                        _leaderBoard.AddLeader(newHigthScorePrompt.Text, _context.Scores, playedTime);
                        GameOverMessage();
                    };
                    newHigthScorePrompt.Cancelled += (_, __) => GameOverMessage();

                    GameRoot.ScreenManager.AddScreen(newHigthScorePrompt, null);
                }
                else
                {
                    GameOverMessage();
                }
            }
        }

        private void GameOverMessage()
        {
            const string message = "GAME OVER?\nA button, Space, Enter = Restart\nB button, Esc = Exit";
            var msg = new MessageBoxScreen(message);

            msg.Accepted += (_, __) => LoadingScreen.Load(GameRoot.ScreenManager, false, null, new StarScreen(), new GamePlayScreen());
            msg.Cancelled += (_, __) => LoadingScreen.Load(GameRoot.ScreenManager, false, null, new StarScreen(), new MainMenuScreen());

            GameRoot.ScreenManager.AddScreen(msg, null);
        }
    }
}
