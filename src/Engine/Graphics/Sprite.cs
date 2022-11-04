using Engine.Content;

namespace Engine.Graphics
{
    public sealed class Sprite : ContentObject
    {
        public Sprite(float height, float width)
        {
            Height = height;
            Width = width;
        }

        public float Height { get; }
        public float Width { get; }
    }
}
