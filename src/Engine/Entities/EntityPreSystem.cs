using System.Linq;

namespace Engine.Entities
{
    public sealed class EntityPreSystem : IGamePlaySystem
    {
        private readonly IWorld _world;

        public EntityPreSystem(IWorld world, uint priority)
        {
            _world = world;

            Priority = priority;
        }

        public uint Priority { get; }

        public void Update(float time)
        {
            _world
                .Where(x => x is IUpdatable)
                .Cast<IUpdatable>()
                .Iter(x => x.Update(time));
        }
    }
}
