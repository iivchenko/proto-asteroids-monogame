using System;

namespace Engine.Graphics
{
    public sealed class Sprite
    {
        public Sprite(float height, float width)
        {

            Id = Guid.NewGuid();
            Height = height;
            Width = width;
        }

        public Guid Id { get; }
        public float Height { get; }
        public float Width { get; }
    }
}
