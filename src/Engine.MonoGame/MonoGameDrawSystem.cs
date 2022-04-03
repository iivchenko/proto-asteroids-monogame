using Comora;
using Engine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System.Numerics;

namespace Engine.MonoGame
{
    public sealed class MonoGameDrawSystem : IPainter, IDrawSystemBatcher, IFontService
    {
        private readonly SpriteBatch _batch;
        private readonly ICamera _camera;
        private readonly MonoGameContentProvider _content;

        public MonoGameDrawSystem(
            SpriteBatch batch, 
            ICamera camera,
            MonoGameContentProvider content)
        {
            _batch = batch;
            _camera = camera;
            _content = content;
        }

        public void Begin()
        {
            _batch.Begin(_camera);
        }

        public void End()
        {
            _batch.End();
        }

        public void Draw(Sprite sprite, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color)
        {
            var texture = _content.GetTexture(sprite);
            _batch
                .Draw(
                    texture,
                    position.ToXna(),
                    null,
                    color.ToXna(),
                    rotation,
                    origin.ToXna(),
                    scale.ToXna(),
                    SpriteEffects.None,
                    0);
        }

        public void Draw(Sprite sprite, Rectangle rectagle, Color color)
        {
            var texture = _content.GetTexture(sprite);
            _batch
                .Draw(
                    texture,
                    rectagle.ToXna(),
                    color.ToXna());
        }

        public void Draw(Sprite sprite, Vector2 position, Rectangle source, Vector2 origin, Vector2 scale, float rotation, Color color)
        {
            var texture = _content.GetTexture(sprite);
            _batch
                .Draw(
                    texture, 
                    position.ToXna(),
                    source.ToXna(),
                    color.ToXna(),
                    rotation,
                    origin.ToXna(),
                    scale.ToXna(),
                    SpriteEffects.None,
                    0);
        }

        public void Draw(Sprite sprite, Rectangle destination, Rectangle source, Color color)
        {
            var texture = _content.GetTexture(sprite);
            _batch.Draw(texture, destination.ToXna(), source.ToXna(), color.ToXna());
        }

        public void DrawString(Font font, string text, Vector2 position, Color color)
        {
            var spriteFont = _content.Load<SpriteFont>(font.Id);
            _batch.DrawString(spriteFont, text, position.ToXna(), color.ToXna());
        }
        
        public void DrawString(Font font, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale)
        {
            var spriteFont = _content.Load<SpriteFont>(font.Id);
            _batch.DrawString(spriteFont, text, position.ToXna(), color.ToXna(), rotation, origin.ToXna(), scale, SpriteEffects.None, 0);
        }

        public Size MeasureText(string text, Font font)
        {
            var spriteFont = _content.Load<SpriteFont>(font.Id);
            var size = spriteFont.MeasureString(text);

            return new Size { Width = size.X, Height = size.Y };
        }
    }
}
