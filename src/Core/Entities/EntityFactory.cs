using Engine;
using Engine.Audio;
using Engine.Content;
using Engine.Graphics;
using Engine.Rules;
using System;
using System.Numerics;

namespace Core.Entities
{
    public sealed class EntityFactory : IEntityFactory
    {
        private const int TinyAsteroidMinSpeed = 400;
        private const int TinyAsteroidMaxSpeed = 500;
        private const int TinyAsteroidMinRotationSpeed = 25;
        private const int TinyAsteroidMaxRotationSpeed = 75;

        private const int SmallAsteroidMinSpeed = 200;
        private const int SmallAsteroidMaxSpeed = 300;
        private const int SmallAsteroidMinRotationSpeed = 25;
        private const int SmallAsteroidMaxRotationSpeed = 75;

        private const int MediumAsteroidMinSpeed = 100;
        private const int MediumAsteroidMaxSpeed = 200;
        private const int MediumAsteroidMinRotationSpeed = 15;
        private const int MediumAsteroidMaxRotationSpeed = 45;

        private const int BigAsteroidMinSpeed = 50;
        private const int BigAsteroidMaxSpeed = 100;
        private const int BigAsteroidMinRotationSpeed = 5;
        private const int BigAsteroidMaxRotationSpeed = 25;

        private readonly SpriteSheet _spriteSheet;
        private readonly Sound _lazer;
        private readonly Sound _explosion;
        private readonly IProjectileFactory _projectileFactory;
        private readonly IEventPublisher _publisher;
        private readonly IPainter _draw;
        private readonly IAudioPlayer _player;
        private readonly IContentProvider _content;

        private readonly Random _random;

        public EntityFactory(
            IContentProvider content,
            IProjectileFactory projectileFactory,
            IEventPublisher eventService,
            IPainter draw,
            IAudioPlayer player)
        {
            _content = content;
            _spriteSheet = content.Load<SpriteSheet>("SpriteSheets/Asteroids.sheet");
            _lazer = content.Load<Sound>("Sounds/laser.sound");
            _explosion = content.Load<Sound>("Sounds/asteroid-explosion.sound");
            _projectileFactory = projectileFactory;
            _publisher = eventService;
            _draw = draw;
            _player = player;

            _random = new Random();
        }

        public Ship CreateShip(Vector2 position)
        {
            const float MaxSpeed = 600.0f;
            const float Acceleration = 20.0f;
            const float MaxRotation = 290.0f;
            const float MaxAngularAcceleration = 30.0f;
            const float Scale = 4.0f;

            var spriteName = _content.GetFiles("Sprites/PlayerShips").RandomPick();
            var sprite = _content.Load<Sprite>(spriteName);
            var laserSpriteName = _content.GetFiles("Sprites/Lasers").RandomPick();
            var laserSprite = _content.Load<Sprite>(laserSpriteName);

            var trailSpriteName = $"fire{_random.Next(0, 20):D2}";

            var trailSprite = _spriteSheet[trailSpriteName];
            var debri = new[] { _spriteSheet["scratch1"], _spriteSheet["scratch2"], _spriteSheet["scratch3"] };
            var reload = TimeSpan.FromMilliseconds(500);
            var weapon = new Weapon(new Vector2(0, -sprite.Width / 2), reload, _projectileFactory, _publisher, _player, laserSprite, _lazer);
            var trails = new[]
            {
                new ShipTrail(trailSprite, new Vector2(-35, 28), new Vector2(trailSprite.Width / 2, 0), _draw),
                new ShipTrail(trailSprite, new Vector2(35, 28), new Vector2(trailSprite.Width / 2, 0), _draw)
            };

            return new Ship(_draw, _publisher, sprite, debri, weapon, trails, MaxSpeed, Acceleration, MaxRotation.AsRadians(), MaxAngularAcceleration.AsRadians())
            {
                Position = position,
                Scale = new Vector2(Scale)
            };
        }
        public Asteroid CreateAsteroid(AsteroidType type, Vector2 position, float direction)
        {
            Sprite sprite;
            int speedX;
            int speedY;
            int rotationSpeed;
            Vector2 velocity;
            float scale = 4;

            switch (type)
            {
                case AsteroidType.Tiny:
                    sprite = _content.Load<Sprite>("Sprites/Asteroids/Tiny/AsteroidTiny01");                    
                    speedX = _random.Next(TinyAsteroidMinSpeed, TinyAsteroidMaxSpeed);
                    speedY = _random.Next(TinyAsteroidMinSpeed, TinyAsteroidMaxSpeed);
                    rotationSpeed = _random.Next(TinyAsteroidMinRotationSpeed, TinyAsteroidMaxRotationSpeed).AsRadians() * _random.NextDouble() > 0.5 ? 1 : -1;
                    velocity = direction.ToDirection() * new Vector2(speedX, speedY);
                    break;

                case AsteroidType.Small:
                    sprite = _content.Load<Sprite>("Sprites/Asteroids/Small/AsteroidSmall01");
                    speedX = _random.Next(SmallAsteroidMinSpeed, SmallAsteroidMaxSpeed);
                    speedY = _random.Next(SmallAsteroidMinSpeed, SmallAsteroidMaxSpeed);
                    rotationSpeed = _random.Next(SmallAsteroidMinRotationSpeed, SmallAsteroidMaxRotationSpeed).AsRadians() * _random.NextDouble() > 0.5 ? 1 : -1;
                    velocity = direction.ToDirection() * new Vector2(speedX, speedY);
                    break;

                case AsteroidType.Medium:
                    sprite = _content.Load<Sprite>("Sprites/Asteroids/Medium/AsteroidMedium01");
                    speedX = _random.Next(MediumAsteroidMinSpeed, MediumAsteroidMaxSpeed);
                    speedY = _random.Next(MediumAsteroidMinSpeed, MediumAsteroidMaxSpeed);
                    rotationSpeed = _random.Next(MediumAsteroidMinRotationSpeed, MediumAsteroidMaxRotationSpeed).AsRadians() * _random.NextDouble() > 0.5 ? 1 : -1;
                    velocity = direction.ToDirection() * new Vector2(speedX, speedY);
                    break;

                case AsteroidType.Big:
                    sprite = _content.Load<Sprite>("Sprites/Asteroids/Big/AsteroidBig01");
                    speedX = _random.Next(BigAsteroidMinSpeed, BigAsteroidMaxSpeed);
                    speedY = _random.Next(BigAsteroidMinSpeed, BigAsteroidMaxSpeed);
                    rotationSpeed = _random.Next(BigAsteroidMinRotationSpeed, BigAsteroidMaxRotationSpeed).AsRadians() * _random.NextDouble() > 0.5 ? 1 : -1;
                    velocity = direction.ToDirection() * new Vector2(speedX, speedY);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown asteroid type {type}!");
            }
            var debri = _content.Load<Sprite>("Sprites/Asteroids/Tiny/AsteroidTiny01"); // TODO: Create own asteroid debri

            return new Asteroid(_draw, _player, _publisher, type, sprite, debri, _explosion, velocity, new Vector2(scale), rotationSpeed)
            {
                Position = position
            };
        }
    }
}
