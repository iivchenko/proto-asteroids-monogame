using KenneyAsteroids.Engine.Screens;

namespace KenneyAsteroids.Core.Screens.GamePlay.PlayerControllers
{
    public interface IPlayerController
    {
        void Handle(InputState input);
    }
}
