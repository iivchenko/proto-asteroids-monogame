namespace KenneyAsteroids.Engine.Audio
{
    public interface IAudioPlayer
    {
        void Play(Sound sound, float pitch = 0.0f);
    }
}
