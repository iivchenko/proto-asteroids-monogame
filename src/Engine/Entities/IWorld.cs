using System.Collections.Generic;

namespace Engine.Entities
{
    public interface IWorld : IEnumerable<IEntity>
    {
        void Add(params IEntity[] entities);
        void Remove(params IEntity[] entities);
        void Commit();
        void Free();
    }
}