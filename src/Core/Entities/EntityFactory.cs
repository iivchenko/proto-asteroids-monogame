using Core.Screens.GamePlay;
using Engine;
using Engine.Audio;
using Engine.Collisions;
using Engine.Content;
using Engine.Graphics;
using Engine.Rules;
using System;
using System.Linq;
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

        private readonly Sound _lazer;
        private readonly Sound _explosion;
        private readonly IProjectileFactory _projectileFactory;
        private readonly IEventPublisher _publisher;
        private readonly IPainter _draw;
        private readonly IAudioPlayer _player;
        private readonly IContentProvider _content;
        private readonly ICollisionService _collisionService;

        private readonly Random _random;

        public EntityFactory(
            IContentProvider content,
            ICollisionService collisionService,
            IProjectileFactory projectileFactory,
            IEventPublisher eventService,
            IPainter draw,
            IAudioPlayer player)
        {
            _content = content;
            _collisionService = collisionService;
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

            var spriteName = _content.GetFiles("Sprites/PlayerShips").RandomPick();
            var sprite = _content.Load<Sprite>(spriteName);
            var laserSpriteName = _content.GetFiles("Sprites/Lasers").RandomPick();
            var laserSprite = _content.Load<Sprite>(laserSpriteName);

            var trailSpriteName = _content.GetFiles("Sprites/Trails").RandomPick();
            var trailSprite = _content.Load<Sprite>(trailSpriteName);
            var debris = _content.GetFiles("Sprites/Debris").Select(_content.Load<Sprite>).ToArray(); var reload = TimeSpan.FromMilliseconds(500);
            var weapon = new Weapon(new Vector2(0, -(sprite.Width * GameRoot.Scale) / 2), reload, _projectileFactory, _publisher, _player, laserSprite, _lazer, WeaponState.Idle, GameTags.Player);

            var xoffset = (sprite.Width * GameRoot.Scale / 2.0f) * 0.65f;
            var yoffset = sprite.Height * GameRoot.Scale / 2.0f;

            var trails = new[]
            {
                new ShipTrail(trailSprite, new Vector2(-xoffset, yoffset), new Vector2(trailSprite.Width / 2, 0), new Vector2(GameRoot.Scale), _draw),
                new ShipTrail(trailSprite, new Vector2(xoffset, yoffset), new Vector2(trailSprite.Width / 2, 0), new Vector2(GameRoot.Scale), _draw)
            };

            var ship = new Ship(_draw, _publisher, sprite, debris, weapon, trails, _player, _explosion, MaxSpeed, Acceleration, MaxRotation.AsRadians(), MaxAngularAcceleration.AsRadians())
            {
                Position = position,
                Scale = new Vector2(GameRoot.Scale)
            };

            _collisionService.RegisterBody(ship, sprite);

            return ship;
        }
        public Asteroid CreateAsteroid(AsteroidType type, Vector2 position, float direction)
        {
            Sprite sprite;
            int speedX;
            int speedY;
            int rotationSpeed;
            Vector2 velocity;

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

            var asteroid = new Asteroid(_draw, _player, _publisher, type, sprite, debri, _explosion, velocity, new Vector2(GameRoot.Scale), rotationSpeed)
            {
                Position = position
            };

            _collisionService.RegisterBody(asteroid, sprite);

            return asteroid;
        }

        public Ufo CreateUfo(Vector2 position, float direction)
        {
            const float MaxSpeed = 400.0f;
            var spriteName = _content.GetFiles("Sprites/Ufos").RandomPick();
            var sprite = _content.Load<Sprite>(spriteName);
            var blasterSpriteName = _content.GetFiles("Sprites/Blasters").RandomPick();
            var blasterSprite = _content.Load<Sprite>(blasterSpriteName);

            var debris = _content.GetFiles("Sprites/Debris").Select(_content.Load<Sprite>).ToArray(); 
            var reload = TimeSpan.FromMilliseconds(1500);
            var weapon = new Weapon(new Vector2(0, -(sprite.Width * GameRoot.Scale) / 2), reload, _projectileFactory, _publisher, _player, blasterSprite, _lazer, WeaponState.Reload, GameTags.Enemy);

            var xoffset = (sprite.Width * GameRoot.Scale / 2.0f);
            var yoffset = sprite.Height * GameRoot.Scale / 2.0f;

            var velocity = direction.ToDirection() * new Vector2(MaxSpeed, MaxSpeed);
            var ufo = new Ufo(_draw, _publisher, sprite, debris, _player, _explosion, weapon, velocity)
            {
                Position = position,
                Scale = new Vector2(GameRoot.Scale)
            };

            _collisionService.RegisterBody(ufo, sprite);

            return ufo;
        }
    }
}
