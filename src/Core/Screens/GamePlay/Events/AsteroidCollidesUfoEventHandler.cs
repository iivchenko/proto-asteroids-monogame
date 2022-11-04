using Core.Entities;

namespace Core.Screens.GamePlay.Events
{
    public sealed class AsteroidCollidesUfoEventHandler : BaseCollisionEventHandler<Asteroid, Ufo>
    {
        protected override bool ExecuteConditionInternal(Asteroid asteroid, Ufo ufo)
            => ufo.State == UfoState.Alive && asteroid.State == AsteroidState.Alive;

        protected override void ExecuteActionInternal(Asteroid asteroid, Ufo _)
            => asteroid.Destroy();
    }
}
