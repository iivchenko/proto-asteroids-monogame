namespace Engine.Entities
{
    public sealed class EntityPostSystem : IGamePlaySystem
    {
        private readonly IWorld _world;

        public EntityPostSystem(IWorld world, uint priority)
        {
            _world = world;

            Priority = priority;
        }

        public uint Priority { get; }

        public void Update(float time)
        {
            _world.Commit();
        }
    }
}
