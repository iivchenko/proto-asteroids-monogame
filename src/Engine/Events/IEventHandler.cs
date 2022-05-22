namespace Engine.Events
{
    public interface IEventHandler<TEvent>
        where TEvent : IEvent
    {
        bool ExecuteCondition(TEvent @event);

        void ExecuteAction(TEvent @event);
    }
}
