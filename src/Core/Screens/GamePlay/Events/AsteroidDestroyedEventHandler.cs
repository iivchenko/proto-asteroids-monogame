using Core.Entities;
using Engine.Collisions;
using Engine.Entities;
using Engine.Events;

namespace Core.Screens.GamePlay.Events
{
    public static partial class EntitiesRules
    {

        public static partial class WhenAsteoidDestroyed
        {
            public sealed class AsteroidDestroyedEventHandler : IEventHandler<AsteroidDestroyedEvent>
            {
                private readonly IWorld _world;
                private readonly ICollisionService _collisionService;

                public AsteroidDestroyedEventHandler(IWorld world, ICollisionService collisionService)
                {
                    _world = world;
                    _collisionService = collisionService;
                }

                public bool ExecuteCondition(AsteroidDestroyedEvent @event) => true;

                public void ExecuteAction(AsteroidDestroyedEvent @event)
                {
                    _world.Remove(@event.Asteroid);
                    _collisionService.UnregisterBody(@event.Asteroid);
                }
            }
        }
    }
}
