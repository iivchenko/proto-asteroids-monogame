using System.Numerics;

using XVector = Microsoft.Xna.Framework.Vector2;
using XRect = Microsoft.Xna.Framework.Rectangle;
using XColor = Microsoft.Xna.Framework.Color;

namespace Engine.MonoGame
{
    public static class MonoGameExtensions
    {
        public static XColor ToXna(this Color color)
        {
            return new XColor(color.Red, color.Green, color.Blue, color.Alpha);
        }

        public static XRect ToXna(this Rectangle rect)
        {
            return new XRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static XRect? ToXna(this Rectangle? rect)
        {
            return rect.HasValue
                ? new XRect(rect.Value.X, rect.Value.Y, rect.Value.Width, rect.Value.Height)
                : new XRect?();
        }

        internal static XVector ToXna(this Vector2 vector)
        {
            return new XVector(vector.X, vector.Y);
        }
    }
}
