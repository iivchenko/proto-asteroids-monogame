using Engine.Graphics;

namespace Engine.Collisions
{
    public interface ICollisionService
    {
        Color[] ReadBodyPixels(IBody body);

        void RegisterBody(IBody body, Sprite sprite);

        void UnregisterBody(IBody body);
    }
}
