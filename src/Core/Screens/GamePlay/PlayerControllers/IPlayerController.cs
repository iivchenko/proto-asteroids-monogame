using Engine.Screens;

namespace Core.Screens.GamePlay.PlayerControllers
{
    public interface IPlayerController
    {
        void Handle(InputState input);
    }
}
