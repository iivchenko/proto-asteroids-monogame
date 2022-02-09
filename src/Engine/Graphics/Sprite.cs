using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace Engine.Graphics
{
    public sealed class Sprite
    {
        private readonly Texture2D _texture;
        private readonly Rectangle _sourceRectangle;

        public Sprite(Texture2D texture, Rectangle? sourceRectangle)
        {
            Id = Guid.NewGuid();
            _texture = texture;

            if (sourceRectangle.HasValue)
            {
                _sourceRectangle = sourceRectangle.Value;
                Height = _sourceRectangle.Height;
                Width = _sourceRectangle.Width;
            }
            else
            {
                Height = texture.Height;
                Width = texture.Width;

                _sourceRectangle = new Rectangle(0, 0, (int)Width, (int)Height);
            }
        }

        public Sprite(Texture2D texture)
            : this(texture, null)
        {
        }

        public Guid Id { get; }
        public float Height { get; }
        public float Width { get; }
        public Texture2D Texture => _texture;
        public Rectangle? SourceRectangle => _sourceRectangle;

        public Color[] ReadData()
        {
            var data = new Microsoft.Xna.Framework.Color[(int)Width * (int)Height];
            var rect = new Microsoft.Xna.Framework.Rectangle(_sourceRectangle.X, _sourceRectangle.Y, _sourceRectangle.Width, _sourceRectangle.Height);
            _texture.GetData(0, rect, data, 0, data.Length);

            return data.Select(color => new Color(color.R, color.G, color.B, color.A)).ToArray();
        }
    }
}
