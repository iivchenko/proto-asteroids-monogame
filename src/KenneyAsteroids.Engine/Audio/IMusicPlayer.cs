namespace KenneyAsteroids.Engine.Audio
{
    public interface IMusicPlayer
    {
        void Play(Music music);
        void Play(Music[] musics);
        void Pause();
        void Resume();
        void Stop();
    }
}
