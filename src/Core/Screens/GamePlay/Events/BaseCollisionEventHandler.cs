using Engine.Events;
using Engine.Collisions;
using System;

namespace Core.Screens.GamePlay.Events
{
    public abstract class BaseCollisionEventHandler<TBody1, TBody2> : IEventHandler<BodiesCollideEvent>
    {
        public bool ExecuteCondition(BodiesCollideEvent @event)
        {
            return (@event.Body1, @event.Body2)
               switch
            {
                (TBody1 body11, TBody2 body12) => ExecuteConditionInternal(body11, body12),
                (TBody2 body22, TBody1 body21) => ExecuteConditionInternal(body21, body22),
                _ => false
            };
        }

        public void ExecuteAction(BodiesCollideEvent @event)
        {
            var (body1, body2) =
                (@event.Body1, @event.Body2)
                   switch
                {
                    (TBody1 body11, TBody2 body12) => (body11, body12),
                    (TBody2 body22, TBody1 body21) => (body21, body22),
                    _ => throw new NotImplementedException()
                };

            ExecuteActionInternal(body1, body2);
        }

        protected abstract bool ExecuteConditionInternal(TBody1 body1, TBody2 body2);

        protected abstract void ExecuteActionInternal(TBody1 body1, TBody2 body2);
    }
}