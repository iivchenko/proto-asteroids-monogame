using System;

namespace KenneyAsteroids.Core.Screens.GamePlay
{
    public sealed class GamePlayContext
    {
        public int Scores { get; set; }

        public int Lifes { get; set; }

        public DateTime StartTime { get; private set; }

        public void Initialize()
        {
            Lifes = 3;
            Scores = 0;
            StartTime = DateTime.Now;
        }
    }
}
