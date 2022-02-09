using System;

namespace Engine.Audio
{
    public sealed class Sound 
    {
        public Sound()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
    }
}
