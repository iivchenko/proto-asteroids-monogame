namespace KenneyAsteroids.Engine
{
    public interface IGamePlaySystem : IUpdatable
    {
        /// <summary>
        /// The lower number the higher priority. 
        /// 0 is the highest priority
        /// System with higher priority should be handled first
        /// </summary>
        uint Priority { get; }
    }
}
