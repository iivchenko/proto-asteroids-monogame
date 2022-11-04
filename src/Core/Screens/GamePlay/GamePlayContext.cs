using System;

namespace Core.Screens.GamePlay
{
    public sealed class GamePlayContext
    {
        public int Scores { get; set; }

        public int Lifes { get; set; }

        public DateTime StartTime { get; private set; }

        public Timer2 NextAsteroidSpawn { get; set; }

        public Timer2 NextSpeedUp { get; set; }

        public Timer2 NextUfoSpawn { get; set; }

        public Timer2 NextHazardSpawn { get; set; }

        public void Initialize()
        {
            Lifes = 7;
            Scores = 0;
            StartTime = DateTime.Now;

            NextAsteroidSpawn = new Timer2(5);
            NextUfoSpawn = new Timer2(45);
            NextHazardSpawn = new Timer2(35);

            NextSpeedUp = new Timer2(60, 10);
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

    public sealed class Timer2
    {
        private readonly bool _isIninite;

        private float _currentTime;
        private int _count;

        public Timer2(float time)
        {
            RecurintTime = _currentTime = time;
            _count = -1;
            _isIninite = true;
        }

        public Timer2(float time, int count)
        {
            RecurintTime = _currentTime = time;
            _count = count;
            _isIninite = false;
        }

        public float RecurintTime { get; set; }

        public bool Update(float time)
        {
            if (_isIninite)
            {
                _currentTime -= time;

                if (_currentTime <= 0)
                {
                    _currentTime = RecurintTime;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (_count > 0)
            {
                _currentTime -= time;

                if (_currentTime <= 0)
                {
                    _currentTime = RecurintTime;
                    _count--;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
