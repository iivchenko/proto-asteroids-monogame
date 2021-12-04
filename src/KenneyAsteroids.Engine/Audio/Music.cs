using System;

namespace KenneyAsteroids.Engine.Audio
{
    public sealed class Music
    {
        public Music()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
    }
}
