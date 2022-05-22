using System;

namespace Core.Screens.GamePlay
{
    public sealed class GamePlayContext
    {
        public int Scores { get; set; }

        public int Lifes { get; set; }

        public DateTime StartTime { get; private set; }

        public AsteroidContext AsteroidContext { get; set; }

        public UfoContext UfoContext { get; set; }

        public HazardContext HazardContext { get; set; }

        public void Initialize()
        {
            Lifes = 3;
            Scores = 0;
            StartTime = DateTime.Now;

            AsteroidContext = new AsteroidContext(3, 1, 60, 3);
            UfoContext = new UfoContext(45);
            HazardContext = new HazardContext(35);
        }
    }

    public sealed class AsteroidContext
    {
        private float _timeToNextAsteroid;
        private float _currentTimetoNextAsteroid;
        private float _timeToNextAsteroidDecrease;
        private float _timeToDecrease;
        private float _currentTimeToDecrease;

        private int _numberOfDecreases;

        public AsteroidContext(
            float timeToNextAsteroid,
            float timeToNextAsteroidDecrease,
            float timeToDecrease,
            int numberOfDecreases)
        {
            _timeToNextAsteroid = _currentTimetoNextAsteroid = timeToNextAsteroid;
            _timeToNextAsteroidDecrease = timeToNextAsteroidDecrease;
            _timeToDecrease = _currentTimeToDecrease = timeToDecrease;
            _numberOfDecreases = numberOfDecreases;
        }

        public bool Update(float time)
        {
            _currentTimetoNextAsteroid -= time;

            if (_numberOfDecreases > 0)
            {
                _currentTimeToDecrease -= time;

                if (_currentTimeToDecrease < 0)
                {
                    _timeToNextAsteroid -= _timeToNextAsteroidDecrease;
                    _currentTimeToDecrease = _timeToDecrease;
                }
            }

            if (_currentTimetoNextAsteroid <= 0)
            {
                _currentTimetoNextAsteroid = _timeToNextAsteroid;             

                return true;
            }

            return false;
        }
    }

    public sealed class UfoContext
    {
        private float _nextUfoTime;
        private float _currentNextUfoTime;

        public UfoContext(float nextUfoTime)
        {
            _nextUfoTime = _currentNextUfoTime = nextUfoTime;
        }

        public bool Update(float time)
        {
            _currentNextUfoTime -= time;

            if (_currentNextUfoTime < 0)
            {
                _currentNextUfoTime = _nextUfoTime;

                return true;
            }

            return false;
        }
    }

    public sealed class HazardContext
    {
        private float _nextHazardTime;
        private float _currentNextHazardTime;

        public HazardContext(float nextHazardTime)
        {
            _nextHazardTime = _currentNextHazardTime = nextHazardTime;
        }

        public bool Update(float time)
        {
            _currentNextHazardTime -= time;

            if (_currentNextHazardTime < 0)
            {
                _currentNextHazardTime = _nextHazardTime;

                return true;
            }

            return false;
        }
    }
}
