using Core.Events;
using Engine.Entities;
using Engine.Events;

namespace Core.Screens.GamePlay.Events
{
    public static partial class EntitiesRules
    {
        public sealed class EntityCreatedEventHandler : IEventHandler<EntityCreatedEvent>
        {
            private readonly IWorld _world;

            public EntityCreatedEventHandler(IWorld world)
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
}
