using System;

namespace Engine.Audio
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
