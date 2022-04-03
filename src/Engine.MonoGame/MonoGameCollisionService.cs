using Engine.Collisions;
using Engine.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Engine.MonoGame
{
    public sealed class MonoGameCollisionService : ICollisionService
    {
        private readonly MonoGameContentProvider _content;
        private readonly IDictionary<IBody, Color[]> _bodies;

        public MonoGameCollisionService(MonoGameContentProvider content)
        {
            _content = content;

            _bodies = new Dictionary<IBody, Color[]>();
        }

        public Color[] ReadBodyPixels(IBody body)
        {
            return _bodies.TryGetValue(body, out var value) ? value : new Color[0];
        }

        public void RegisterBody(IBody body, Sprite sprite)
        {
            var texture = _content.GetTexture(sprite);
            var data = new Microsoft.Xna.Framework.Color[texture.Width * texture.Height];

            texture.GetData(data);

            _bodies.Add(body, data.Select(color => new Color(color.R, color.G, color.B, color.A)).ToArray());
        }

        public void UnregisterBody(IBody body)
        {
            _bodies.Remove(body);
        }
    }
}
