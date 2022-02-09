using System.Collections.Generic;

namespace Engine.Graphics
{
    public sealed class SpriteSheet
    {
        private readonly IDictionary<string, Sprite> _sprites;

        public SpriteSheet(IDictionary<string, Sprite> sprites)
        {
            _sprites = sprites;
        }

        public Sprite this [string name] 
        {
            get => _sprites[name];
        }
    }
}
