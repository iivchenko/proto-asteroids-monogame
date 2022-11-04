using Engine.Entities;
using Engine.Events;
using System;

namespace Core.Events
{
    public sealed class EntityCreatedEvent : IEvent
    {
        public EntityCreatedEvent(IEntity entity)
        {
            Entity = entity;

            Id = Guid.NewGuid();
        }

        public IEntity Entity { get; }

        public Guid Id { get; }
    }
}
