namespace Engine.Events
{
    public interface IEventPublisher
    {
        void Publish<TEvent>(TEvent @event)
            where TEvent : IEvent;
    }
}

