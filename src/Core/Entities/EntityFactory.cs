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

            var shipSpriteName = new[]
                {
                    "playerShip1_blue", "playerShip1_orange", "playerShip1_red", "playerShip1_green",
                    "playerShip2_blue", "playerShip2_orange", "playerShip2_red", "playerShip2_green",
                    "playerShip3_blue", "playerShip3_orange", "playerShip3_red", "playerShip3_green"
                }.RandomPick();

            var trailSpriteName = $"fire{_random.Next(0, 20):D2}";
            var lazerSpriteName = new[]
            {
                "laserBlue01", "laserBlue02", "laserBlue03", "laserBlue04", "laserBlue05", "laserBlue06", "laserBlue07", "laserBlue12", "laserBlue13", "laserBlue14", "laserBlue15", "laserBlue16",
                "laserGreen02", "laserGreen03", "laserGreen04", "laserGreen05", "laserGreen06", "laserGreen07", "laserGreen08", "laserGreen09", "laserGreen10", "laserGreen11", "laserGreen12", "laserGreen13",   
                "laserRed01", "laserRed02", "laserRed03", "laserRed04", "laserRed05", "laserRed06", "laserRed07", "laserRed12", "laserRed13", "laserRed14", "laserRed15", "laserRed16"
            }.RandomPick();

            var sprite = _spriteSheet[shipSpriteName];
            var trailSprite = _spriteSheet[trailSpriteName];
            var lazerSprite = _spriteSheet[lazerSpriteName];
            var debri = new[] { _spriteSheet["scratch1"], _spriteSheet["scratch2"], _spriteSheet["scratch3"] };
            var reload = TimeSpan.FromMilliseconds(500);
            var weapon = new Weapon(new Vector2(0, -sprite.Width / 2), reload, _projectileFactory, _publisher, _player, lazerSprite, _lazer);
            var trails = new[]
            {
                new ShipTrail(trailSprite, new Vector2(-35, 28), new Vector2(trailSprite.Width / 2, 0), _draw),
                new ShipTrail(trailSprite, new Vector2(35, 28), new Vector2(trailSprite.Width / 2, 0), _draw)
            };

            return new Ship(_draw, _publisher, sprite, debri, weapon, trails, MaxSpeed, Acceleration, MaxRotation.AsRadians(), MaxAngularAcceleration.AsRadians())
            {
                Position = position
            };
        }
        public Asteroid CreateAsteroid(AsteroidType type, Vector2 position, float direction)
        {
            String spriteName;
            Sprite sprite;
            int speedX;
            int speedY;
            int rotationSpeed;
            Vector2 velocity;
            float scale;

            switch (type)
            {
                case AsteroidType.Tiny:
                    sprite = _content.Load<Sprite>("Sprites/Asteroids/Tiny/AsteroidTiny01");
                    scale = 4;
                    speedX = _random.Next(TinyAsteroidMinSpeed, TinyAsteroidMaxSpeed);
                    speedY = _random.Next(TinyAsteroidMinSpeed, TinyAsteroidMaxSpeed);
                    rotationSpeed = _random.Next(TinyAsteroidMinRotationSpeed, TinyAsteroidMaxRotationSpeed).AsRadians() * _random.NextDouble() > 0.5 ? 1 : -1;
                    velocity = direction.ToDirection() * new Vector2(speedX, speedY);
                    break;

                case AsteroidType.Small:
                    spriteName = new[]
                    {
                        "meteorBrown_small1",
                        "meteorBrown_small2",
                        "meteorGrey_small1",
                        "meteorGrey_small2"
                    }.RandomPick();

                    sprite = _spriteSheet[spriteName];
                    scale = 2.2f;
                    speedX = _random.Next(SmallAsteroidMinSpeed, SmallAsteroidMaxSpeed);
                    speedY = _random.Next(SmallAsteroidMinSpeed, SmallAsteroidMaxSpeed);
                    rotationSpeed = _random.Next(SmallAsteroidMinRotationSpeed, SmallAsteroidMaxRotationSpeed).AsRadians() * _random.NextDouble() > 0.5 ? 1 : -1;
                    velocity = direction.ToDirection() * new Vector2(speedX, speedY);
                    break;

                case AsteroidType.Medium:
                    spriteName = new[]
                    {
                        "meteorGrey_med1",
                        "meteorGrey_med2",
                        "meteorBrown_med1",
                        "meteorBrown_med2"
                    }.RandomPick();

                    sprite = _spriteSheet[spriteName];
                    scale = 1.7f;
                    speedX = _random.Next(MediumAsteroidMinSpeed, MediumAsteroidMaxSpeed);
                    speedY = _random.Next(MediumAsteroidMinSpeed, MediumAsteroidMaxSpeed);
                    rotationSpeed = _random.Next(MediumAsteroidMinRotationSpeed, MediumAsteroidMaxRotationSpeed).AsRadians() * _random.NextDouble() > 0.5 ? 1 : -1;
                    velocity = direction.ToDirection() * new Vector2(speedX, speedY);
                    break;

                case AsteroidType.Big:
                    spriteName = new[]
                    {
                        "meteorBrown_big1",
                        "meteorBrown_big2",
                        "meteorBrown_big3",
                        "meteorBrown_big4",
                        "meteorGrey_big1",
                        "meteorGrey_big2",
                        "meteorGrey_big3",
                        "meteorGrey_big4"
                    }.RandomPick();

                    sprite = _spriteSheet[spriteName];
                    scale = 1;
                    speedX = _random.Next(BigAsteroidMinSpeed, BigAsteroidMaxSpeed);
                    speedY = _random.Next(BigAsteroidMinSpeed, BigAsteroidMaxSpeed);
                    rotationSpeed = _random.Next(BigAsteroidMinRotationSpeed, BigAsteroidMaxRotationSpeed).AsRadians() * _random.NextDouble() > 0.5 ? 1 : -1;
                    velocity = direction.ToDirection() * new Vector2(speedX, speedY);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown asteroid type {type}!");
            }
            var debri = _spriteSheet["meteorBrown_tiny1"];

            return new Asteroid(_draw, _player, _publisher, type, sprite, debri, _explosion, velocity, new Vector2(scale), rotationSpeed)
            {
                Position = position
            };
        }
    }
}
