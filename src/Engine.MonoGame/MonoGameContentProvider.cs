using Engine.Audio;
using Engine.Content;
using Engine.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine.MonoGame
{
    public sealed class MonoGameContentProvider : IContentProvider
    {
        private readonly ContentManager _content;
        private readonly IDictionary<Guid, string> _map;

        public MonoGameContentProvider(ContentManager content)
        {
            _content = content;
            _map = new Dictionary<Guid, string>();
        }

        public IEnumerable<string> GetFiles(string subFolder)
        {
            var index = _content.RootDirectory.Length + 1;

            return
                Directory
                    .GetFiles(Path.Combine(_content.RootDirectory, subFolder))
                    .Select(path =>
                    {
                        var relativePath = path.Substring(index);
                        var file = relativePath.Substring(0, relativePath.Length - 4);

                        return file;
                    }).ToList();
        }

        public TContent Load<TContent>(string path)
            where TContent : class
        {
            var type = typeof(TContent);

            if (type == typeof(Sprite))
            {
                var texture =_content.Load<Texture2D>(path);

                var sprite = new Sprite(texture);

                _map.Add(sprite.Id, path);

                return sprite as TContent;
            }
            else if (type == typeof(SpriteSheet))
            {
                return _content.Load<SpriteSheet>(path) as TContent;
            }
            else if (type == typeof(Sound))
            {
                var sound = new Sound();

                _content.Load<SoundEffect>(path);

                _map.Add(sound.Id, path);

                return sound as TContent;
            }
            else if (type == typeof(Font))
            {
                var spriteFont = _content.Load<SpriteFont>(path);
                var font = new Font(spriteFont.LineSpacing);

                _map.Add(font.Id, path);

                return font as TContent;
            }
            else if (type == typeof(Music))
            {
                var music = new Music();

                _content.Load<Song>(path);

                _map.Add(music.Id, path);

                return music as TContent;
            }
            else
            {
                throw new System.Exception($"Unknown content type {type.Name}!!");
            }
        }

        internal TContent Load<TContent>(Guid id)
        {
            if (_map.TryGetValue(id, out var path))
            {
                return _content.Load<TContent>(path);
            }

            throw new InvalidOperationException($"Specified id doesn't exist '{id}'!");
        }
    }
}
