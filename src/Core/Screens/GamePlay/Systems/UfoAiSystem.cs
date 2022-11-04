using Core.Entities;
using Engine;
using Engine.Entities;
using System.Linq;

namespace Core.Screens.GamePlay.Systems
{
    public sealed class UfoAiSystem : IGamePlaySystem
    {
        private readonly IWorld _world;

        public UfoAiSystem(IWorld world, uint priority)
        {
            _world = world;

            Priority = priority;
        }

        public uint Priority { get; }

        public void Update(float time)
        {
            var player =
                _world
                    .Where(x => x is Ship)
                    .Cast<Ship>()
                    .First();

            _world
                .Where(x => x is Ufo)
                .Cast<Ufo>()
                .Where(ufo => ufo.State == UfoState.Alive)
                .Where(ufo => ufo.CanFire)
                .Iter(ufo => ufo.Fire(player.Position - ufo.Position));
        }
    }
}
