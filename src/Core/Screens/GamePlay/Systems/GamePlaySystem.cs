using Core.Entities;
using Engine;
using Engine.Audio;
using Engine.Content;
using Engine.Entities;
using Engine.Graphics;
using System;
using System.Linq;
using System.Numerics;

namespace Core.Screens.GamePlay.Systems
{
    public sealed class GamePlaySystem : IGamePlaySystem
    {
        private readonly GamePlayContext _context;
        private readonly IWorld _world;
        private readonly IViewport _viewport;
        private readonly IEntityFactory _entityFactory;
        private readonly IContentProvider _content;
        private readonly IAudioPlayer _player;
        private readonly Random _random;

        public GamePlaySystem(
            GamePlayContext context,
            IWorld world,
            IViewport viewport,
            IEntityFactory entityFactory,
            IContentProvider content,
            IAudioPlayer player)
        {
            _context = context;
            _world = world;
            _viewport = viewport;
            _entityFactory = entityFactory;
            _content = content;
            _player = player;

            _random = new Random();
        }

        public uint Priority => 1;

        public void Update(float time)
        {
            if (_context.NextAsteroidSpawn.Update(time)) CreateAsteroid();            
            if (_context.NextUfoSpawn.Update(time)) CreateUfo();
            if (_context.NextHazardSpawn.Update(time)) CreateHazard();

            if (_context.NextSpeedUp.Update(time)) SpeedUpGame();
        }

        public void CreateAsteroid()
        {
            var x = 0;
            var y = 0;

            switch (_random.Next(0, 4))
            {
                case 0: // Up -> Down
                    x = _random.Next(0, (int)_viewport.Width);
                    y = 0;
                    break;

                case 1: // Right -> Left
                    x = (int)_viewport.Width;
                    y = _random.Next(0, (int)_viewport.Height);
                    break;

                case 2: // Down -> UP
                    x = _random.Next(0, (int)_viewport.Width);
                    y = (int)_viewport.Height;
                    break;

                case 3: // Left -> Right
                    x = 0;
                    y = _random.Next(0, (int)_viewport.Height);
                    break;
            }

            var position = new Vector2(x, y);
            var direction = _random.Next(0, 360).AsRadians();
            var type = new[] { AsteroidType.Tiny, AsteroidType.Small, AsteroidType.Medium, AsteroidType.Big }.RandomPick();
            var asteroid = _entityFactory.CreateAsteroid(type, position, direction);

            _world.Add(asteroid);
        }

        public void SpeedUpGame()
        {
            _context.NextAsteroidSpawn.RecurintTime = _context.NextAsteroidSpawn.RecurintTime * 0.85f;
            _context.NextUfoSpawn.RecurintTime = _context.NextUfoSpawn.RecurintTime * 0.89f;
            _context.NextHazardSpawn.RecurintTime = _context.NextHazardSpawn.RecurintTime * 0.87f;
        }

        public void CreateUfo()
        {
            var x = 0;
            var y = 0;

            switch (_random.Next(0, 4))
            {
                case 0: // Up -> Down
                    x = _random.Next(0, (int)_viewport.Width);
                    y = 0;
                    break;

                case 1: // Right -> Left
                    x = (int)_viewport.Width;
                    y = _random.Next(0, (int)_viewport.Height);
                    break;

                case 2: // Down -> UP
                    x = _random.Next(0, (int)_viewport.Width);
                    y = (int)_viewport.Height;
                    break;

                case 3: // Left -> Right
                    x = 0;
                    y = _random.Next(0, (int)_viewport.Height);
                    break;
            }

            var position = new Vector2(x, y);
            var direction = _random.Next(0, 360).AsRadians();
            var ufo = _entityFactory.CreateUfo(position, direction);

            _world.Add(ufo);

            var sound = _content.Load<Sound>("Sounds/enemy-spawn.sound");

            _player.Play(sound);
        }

        public void CreateHazard()
        {
            Asteroid Create(Vector2 position, Vector2 target)
            {
                var direction = Vector2.Normalize(target - position).ToRotation();
                return _entityFactory.CreateAsteroid(AsteroidType.Tiny, position, direction);
            }

            var player = _world.First(entity => entity is Ship) as Ship;
            var target = player.Position;
            _world.Add(
                Create(new Vector2(_viewport.Width / 2, 0), target),
                Create(new Vector2(_viewport.Width, _viewport.Height / 2), target),
                Create(new Vector2(_viewport.Width / 2, _viewport.Height), target),
                Create(new Vector2(0, _viewport.Height / 2), target));
        }
    }
}
