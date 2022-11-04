using Engine.Audio;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.MonoGame
{
    public sealed class MonoGameSoundSystem : IAudioPlayer, IMusicPlayer, IDisposable
    {
        private readonly IOptionsMonitor<AudioSettings> _settings;
        private readonly MonoGameContentProvider _content;
        private IList<Song> _songs;

        public MonoGameSoundSystem(
            IOptionsMonitor<AudioSettings> settings,
            MonoGameContentProvider content)
        {
            _settings = settings;
            _content = content;
            _songs = new List<Song>();

            _settings.OnChange(s => MediaPlayer.Volume = s.MusicVolume);

            MediaPlayer.ActiveSongChanged += OnStateChanged;

        }

        public void Play(Sound sound, float pitch = 0.0f)
        {
            var sfx = _content.Load<SoundEffect>(sound.Id);
            sfx.Play(_settings.CurrentValue.SfxVolume, pitch, 0.0f);
        }

        public void Play(Music music)
        {
            _songs = new List<Song> { _content.Load<Song>(music.Id) };

            Play();
        }

        public void Play(Music[] musics)
        {
            _songs =
                musics
                    .Select(x => x.Id)
                    .Select(id => _content.Load<Song>(id))
                    .ToList();
            Play();
        }

        public void Pause()
        {
            MediaPlayer.Pause();
        }

        public void Resume()
        {
            MediaPlayer.Resume();
        }

        public void Stop()
        {
            MediaPlayer.Stop();
        }

        public void Dispose()
        {
            MediaPlayer.ActiveSongChanged -= OnStateChanged;
        }

        private void Play()
        {
            if (_songs.Count == 0)
                return;

            var song = _songs.First();

            _songs.Remove(song);
            _songs.Add(song);

            MediaPlayer.Play(song);
        }

        private void OnStateChanged(object sender, EventArgs e)
        {

            Play();
        }
    }
}
