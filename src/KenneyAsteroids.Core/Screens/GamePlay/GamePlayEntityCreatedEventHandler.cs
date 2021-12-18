using KenneyAsteroids.Core.Events;
using KenneyAsteroids.Engine.Entities;
using KenneyAsteroids.Engine.Rules;

namespace KenneyAsteroids.Core.Screens.GamePlay
{
    public sealed class GamePlayEntityCreatedEventHandler : IRule<EntityCreatedEvent>
    {
        private readonly IWorld _world;

        public GamePlayEntityCreatedEventHandler(IWorld world)
        {
            _world = world;
        }

        public bool ExecuteCondition(EntityCreatedEvent @event) => true;

        public void ExecuteAction(EntityCreatedEvent @event)
        {
            _world.Add(@event.Entity);
        }
    }
}
