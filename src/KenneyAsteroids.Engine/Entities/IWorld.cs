using System.Collections.Generic;

namespace KenneyAsteroids.Engine.Entities
{
    public interface IWorld : IEnumerable<IEntity>
    {
        void Add(params IEntity[] entities);
        void Remove(params IEntity[] entities);
        void Commit();
        void Free();
    }
}