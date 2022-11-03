using System;

namespace Engine.Content
{
    public abstract class ContentObject
    {
        protected ContentObject()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
    }
}
