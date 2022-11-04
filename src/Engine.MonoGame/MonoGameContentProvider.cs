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
    public sealed class ContentRoot
    {
        public ContentRoot(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }

    public sealed class MonoGameContentProvider : IContentProvider
    {
        private readonly ContentManager _content;
        private readonly IDictionary<Guid, object> _map;
        private readonly string _root;

        public MonoGameContentProvider(ContentManager content, ContentRoot root)
        {
            _content = content;
            _map = new Dictionary<Guid, object>();

            _root = root.Path;
        }

        public IEnumerable<string> GetFiles(string subFolder)
        {
            var root = Path.Combine(_root, _content.RootDirectory);
            var index = root.Length + 1;

            return
                Directory
                    .GetFiles(Path.Combine(root, subFolder))
                    .Select(path =>
                    {
                        var relativePath = path.Substring(index);
                        var file = relativePath.Substring(0, relativePath.Length - 4);

                        return file;
                    }).ToList();
        }

        public TContent Load<TContent>(string path)
            where TContent : ContentObject
        {
            var type = typeof(TContent);

            if (type == typeof(Sprite))
            {
                var texture =_content.Load<Texture2D>(path);

                var sprite = new Sprite(texture.Height, texture.Width);

                _map.Add(sprite.Id, texture);

                return sprite as TContent;
            }
            else if (type == typeof(Sound))
            {
                var sound = new Sound();

                var sfx = _content.Load<SoundEffect>(path);

                _map.Add(sound.Id, sfx);

                return sound as TContent;
            }
            else if (type == typeof(Font))
            {
                var spriteFont = _content.Load<SpriteFont>(path);
                var font = new Font(spriteFont.LineSpacing);

                _map.Add(font.Id, spriteFont);

                return font as TContent;
            }
            else if (type == typeof(Music))
            {
                var music = new Music();

                var song = _content.Load<Song>(path);

                _map.Add(music.Id, song);

                return music as TContent;
            }
            else
            {
                throw new System.Exception($"Unknown content type {type.Name}!!");
            }
        }

        internal Texture2D GetTexture(Sprite sprite)
        {
            return _map[sprite.Id] as Texture2D;
        }

        internal TContent Load<TContent>(Guid id)
        {
            if (_map.TryGetValue(id, out var value))
            {
                return (TContent)value;
            }

            throw new InvalidOperationException($"Specified id doesn't exist '{id}'!");
        }
    }
}
