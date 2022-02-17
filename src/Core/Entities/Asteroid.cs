using Core.Screens.GamePlay;
using Engine;
using Engine.Audio;
using Engine.Collisions;
using Engine.Entities;
using Engine.Graphics;
using Engine.Particles;
using Engine.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Core.Entities
{
    public sealed class Asteroid : IEntity<Guid>, IUpdatable, IDrawable, IBody
    {
        private readonly IPainter _draw;
        private readonly IAudioPlayer _player;
        private readonly IEventPublisher _publisher;

        private readonly Sprite _sprite;
        private readonly Sprite _debri;
        private readonly Sound _explosion;

        private readonly float _rotationSpeed;

        private Vector2 _velocity;

        private IState _state;

        public Asteroid(
            IPainter draw,
            IAudioPlayer player,
            IEventPublisher publisher,
            AsteroidType type,
            Sprite sprite,
            Sprite debri,
            Sound explosion,
            Vector2 velocity,
            Vector2 scale,
            float rotationSpeed)
        {
            _sprite = sprite;
            _debri = debri;
            _explosion = explosion;
            _draw = draw;
            _player = player;
            _publisher = publisher;
            _velocity = velocity;
            _rotationSpeed = rotationSpeed;

            Id = Guid.NewGuid();
            Type = type;
            Origin = new Vector2(_sprite.Width / 2.0f, _sprite.Height / 2.0f);
            Rotation = 0.0f;
            Position = Vector2.Zero;
            Scale = scale;
            Width = _sprite.Width;
            Height = _sprite.Height;
            Data = sprite.ReadData();

            _state = new AliveState(this);
        }

        public Guid Id { get; }
        public IEnumerable<string> Tags => new[] { GameTags.Enemy };
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Color[] Data { get; set; }
        public AsteroidType Type { get; set; }
        public Vector2 Velocity => _velocity;
        public AsteroidState State
        {
            get => _state switch
            {
                AliveState _ => AsteroidState.Alive,
                DestroyState _ => AsteroidState.Destroy,
                DeadState _ => AsteroidState.Dead,
                _ => throw new NotImplementedException()
            };
        }

        void IUpdatable.Update(float time)
        {
            _state.Update(time);
        }

        void IDrawable.Draw(float time)
        {
            _state.Draw(time);
        }

        public void Destroy()
        {
            _state = new DestroyState(this);
        }

        private interface IState
        {
            void Update(float time);

            void Draw(float time);
        }

        private sealed class AliveState : IState
        {
            private readonly Asteroid _asteroid;

            public AliveState(Asteroid asteroid)
            {
                _asteroid = asteroid;
            }

            public void Draw(float time)
            {
                _asteroid
                    ._draw
                        .Draw(
                            _asteroid._sprite,
                            _asteroid.Position,
                            _asteroid.Origin,
                            _asteroid.Scale,
                            _asteroid.Rotation,
                            Colors.White);
            }

            public void Update(float time)
            {
                _asteroid.Position += _asteroid._velocity * time;
                _asteroid.Rotation += _asteroid._rotationSpeed * time;
            }
        }

        private sealed class DestroyState : IState
        {
            private readonly Asteroid _asteroid;
            private readonly ParticleEngine _particleEngine;

            public DestroyState(Asteroid asteroid)
            {
                _asteroid = asteroid;
                _asteroid._player.Play(_asteroid._explosion);
                _particleEngine =
                    Particles
                    .CreateNew()
                    .WithInit(rand =>
                        Enumerable
                            .Range(0, rand.Next(5, 10))
                            .Select(_ =>
                                new Particle
                                {
                                    Angle = rand.Next(0, 360).AsRadians(),
                                    AngularVelocity = rand.Next(5, 100).AsRadians(),
                                    Color = Colors.White,
                                    Position = asteroid.Position,
                                    Scale = asteroid.Type == AsteroidType.Big ? new Vector2(2) : Vector2.One,
                                    Sprite = _asteroid._debri,
                                    TTL = asteroid.Type == AsteroidType.Big ? 6 : 4,
                                    Velocity = new Vector2(rand.Next(-100, 100), rand.Next(-100, 100)),
                                }))
                            .WithUpdate((rand, time, particle) =>
                            {
                                particle.Position += particle.Velocity * time;
                                particle.Angle += particle.AngularVelocity * time;
                                particle.TTL -= time;
                                particle.Color *= 0.99f;
                                particle.Color = new Color(
                                ClampColor(particle.Color.Red, time),
                                ClampColor(particle.Color.Green, time),
                                ClampColor(particle.Color.Blue, time),
                                ClampColor(particle.Color.Alpha, time));
                            })
                            .Build((int)DateTime.Now.Ticks, _asteroid._draw, time => _asteroid._state = new DeadState(_asteroid));
            }

            public void Draw(float time)
            {
                _particleEngine.Draw(time);
            }

            public void Update(float time)
            {
                _particleEngine.Update(time);
            }

            private static byte ClampColor(byte color, float time)
            {
                return Math.Clamp((byte)(color - color * 1.5 * time), (byte)0, (byte)255);
            }
        }

        private sealed class DeadState : IState
        {
            public DeadState(Asteroid asteroid)
            {
                asteroid._publisher.Publish(new AsteroidDestroyedEvent(asteroid));
            }

            public void Draw(float time) { }

            public void Update(float time) { }
        }
    }

    public sealed class AsteroidDestroyedEvent : IEvent
    {
        public AsteroidDestroyedEvent(Asteroid asteroid)
        {
            Id = Guid.NewGuid();
            Asteroid = asteroid;
        }
        public Guid Id { get; }

        public Asteroid Asteroid { get; }
    }

    public enum AsteroidType
    {
        Tiny,
        Small,
        Medium,
        Big
    }

    public enum AsteroidState 
    {
        Alive,
        Destroy,
        Dead
    }
}
