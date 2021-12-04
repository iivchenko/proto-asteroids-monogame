using System;
using System.Numerics;

using XVector = Microsoft.Xna.Framework.Vector2;

namespace KenneyAsteroids.Engine
{
    public static class VectorExtensions
    {

        public static float ToRotation(this Vector2 direction)
        {
            return MathF.Atan2(direction.X, -direction.Y);
        }

        internal static XVector ToXnaVector(this Vector2 vector)
        {
            return new XVector(vector.X, vector.Y);
        }

        internal static Vector2 ToVector(this XVector vector)
        {
            return new Vector2(vector.X, vector.Y);
        }
    }
}
