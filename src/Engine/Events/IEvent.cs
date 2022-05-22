using System;

namespace Engine.Events
{
    public interface IEvent
    {
        Guid Id { get; }
    }
}
