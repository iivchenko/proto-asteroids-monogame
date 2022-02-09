using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Entities
{
    public sealed class World : IWorld
    {
        private readonly List<IEntity> _entities;
        private readonly List<Modification> _modifications;

        public World()
        {
            _entities = new List<IEntity>();
            _modifications = new List<Modification>();
        }

        public void Add(params IEntity[] entities)
        {
            entities
                .Select(x => new Modification { Type = ModificationType.Add, Entity = x })
                .Iter(_modifications.Add);
        }

        public void Remove(params IEntity[] entities)
        {
            entities
                .Select(x => new Modification { Type = ModificationType.Remove, Entity = x })
                .Iter(_modifications.Add);
        }

        public void Free()
        {
            _entities.Clear();
            _modifications.Clear();
        }

        public IEnumerator<IEntity> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        public void Commit()
        {
            _modifications.Iter(x =>
            {
                switch (x.Type)
                {
                    case ModificationType.Add:
                        _entities.Add(x.Entity);
                        break;

                    case ModificationType.Remove:
                        _entities.Remove(x.Entity);
                        break;
                }
            });

            _modifications.Clear();
        }

        private enum ModificationType
        {
            Add,
            Remove
        }

        private sealed class Modification
        {
            public IEntity Entity { get; set; }

            public ModificationType Type { get; set; }
        }
    }
}
