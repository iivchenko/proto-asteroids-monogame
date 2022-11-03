using Engine.Content;

namespace Engine.Graphics
{
    public sealed class Font : ContentObject
    {
        public Font(int lineSpacing)
        {
            LineSpacing = lineSpacing;
        }

        public int LineSpacing { get; }
    }
}
