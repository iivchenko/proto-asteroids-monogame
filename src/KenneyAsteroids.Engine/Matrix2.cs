using System.Numerics;

namespace KenneyAsteroids.Engine
{
    public sealed class Matrix2
    {
        public static readonly Matrix2 Identity = new Matrix2(1.0f, 0.0f, 0.0f, 1.0f);

        public Matrix2(float a11, float a12, float a21, float a22)
        {
            A11 = a11;
            A12 = a12;
            A21 = a21; 
            A22 = a22;
        }

        public float A11 { get; }

        public float A12 { get; }

        public float A21 { get; }

        public float A22 { get; }

        public static Matrix2 CreateRotation(float angle)
        {
            return new Matrix2(System.MathF.Cos(angle), -System.MathF.Sin(angle), System.MathF.Sin(angle), System.MathF.Cos(angle));
        }

        public static Vector2 operator* (Matrix2 matrix, Vector2 vector)
        {
            return new Vector2(matrix.A11 * vector.X + matrix.A12 * vector.Y, matrix.A21 * vector.X + matrix.A22 * vector.Y);
        }
    }
}
