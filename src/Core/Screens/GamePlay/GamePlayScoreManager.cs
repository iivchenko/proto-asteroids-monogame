using Core.Entities;
using Engine.Entities;
using System;

namespace Core.Screens.GamePlay
{
    public sealed class GamePlayScoreManager
    {
        public int GetScore(IEntity entity)
            => entity switch
            {
                Asteroid asteroid when asteroid.Type == AsteroidType.Tiny => 25,
                Asteroid asteroid when asteroid.Type == AsteroidType.Small => 20,
                Asteroid asteroid when asteroid.Type == AsteroidType.Medium => 15,
                Asteroid asteroid when asteroid.Type == AsteroidType.Big => 10,
                Ufo _ => 100,
                _ => throw new InvalidOperationException($"Can't calculate scores for {entity}")
            };
    }
}
