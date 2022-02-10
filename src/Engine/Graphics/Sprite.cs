using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace Engine.Graphics
{
    public sealed class Sprite
    {
        private readonly Texture2D _texture;

        public Sprite(Texture2D texture)
        {
            _texture = texture;

            Id = Guid.NewGuid();
            Height = texture.Height;
            Width = texture.Width;
        }

        public Guid Id { get; }
        public float Height { get; }
        public float Width { get; }
        public Texture2D Texture => _texture;

        public Color[] ReadData()
        {
            var data = new Microsoft.Xna.Framework.Color[(int)Width * (int)Height];

            _texture.GetData(data);

            return data.Select(color => new Color(color.R, color.G, color.B, color.A)).ToArray();
        }
    }
}
