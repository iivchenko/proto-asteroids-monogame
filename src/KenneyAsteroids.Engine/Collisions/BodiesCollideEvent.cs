using KenneyAsteroids.Engine.Rules;
using System;

namespace KenneyAsteroids.Engine.Collisions
{
    public sealed class BodiesCollideEvent : IEvent
    {
        public BodiesCollideEvent(IBody body1, IBody body2)
        {
            Id = Guid.NewGuid();
            Body1 = body1;
            Body2 = body2;
        }

        public Guid Id { get; }
        public IBody Body1 { get; }
        public IBody Body2 { get; }
    }
}
