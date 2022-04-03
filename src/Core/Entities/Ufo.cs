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
    public sealed class Ufo : IEntity<Guid>, IUpdatable, IDrawable, IBody
    {
        private readonly IPainter _draw;
        private readonly IEventPublisher _publisher;
        private readonly Sprite _sprite;
        private readonly Sprite[] _debri;
        private readonly Weapon _weapon;
        private readonly IAudioPlayer _audioPlayer;
        private readonly Sound _explosion;

        private readonly Vector2 _velocity;

        private IState _state;

        public Ufo(
            IPainter draw,
            IEventPublisher publisher,
            Sprite sprite,
            Sprite[] debri,
            IAudioPlayer audioPlayer,
            Sound explosion,
            Weapon weapon,
            Vector2 velocity)
        {
            _draw = draw;
            _publisher = publisher;
            _sprite = sprite;
            _debri = debri;
            _weapon = weapon;
            _audioPlayer = audioPlayer;
            _explosion = explosion;
            _velocity = velocity;

            Id = Guid.NewGuid();
            Origin = new Vector2(_sprite.Width / 2.0f, _sprite.Height / 2.0f);
            Position = Vector2.Zero;
            Rotation = 0.0f;
            Scale = Vector2.One;
            Width = _sprite.Width;
            Height = _sprite.Height;

            _state = new AliveState(this);
        }

        public Guid Id { get; }
        public IEnumerable<string> Tags => Enumerable.Empty<string>();
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public bool CanFire { get => _weapon.State == WeaponState.Idle; }
        public UfoState State
        {
            get => _state switch
            {
                AliveState _ => UfoState.Alive,
                DestroyState _ => UfoState.Destroy,
                DeadState _ => UfoState.Dead,
                _ => throw new NotImplementedException()
            };
        }

        public void Fire(Vector2 direction)
        {
            _weapon.Fire(Position, direction.ToRotation());
        }

        public void Destroy()
        {
            _state = new DestroyState(this);
        }

        void IUpdatable.Update(float time)
        {
            _state.Update(time);
        }

        void IDrawable.Draw(float time)
        {
            _state.Draw(time);
        }

        private interface IState
        {
            void Update(float time);

            void Draw(float time);
        }

        private class AliveState : IState
        {
            public readonly Ufo _ufo;

            public AliveState(Ufo ufo)
            {
                _ufo = ufo;
            }

            public virtual void Draw(float time)
            {
                _ufo
                    ._draw
                        .Draw(
                            _ufo._sprite,
                            _ufo.Position,
                            _ufo.Origin,
                            _ufo.Scale,
                            _ufo.Rotation,
                            Colors.White);
            }

            public virtual void Update(float time)
            {
                _ufo._weapon.Update(time);
                _ufo.Position += _ufo._velocity * time;
            }
        }

        private sealed class DestroyState : IState
        {
            private readonly Ufo _ufo;
            private readonly ParticleEngine _particleEngine;

            public DestroyState(Ufo ufo)
            {
                _ufo = ufo;
                _ufo._audioPlayer.Play(_ufo._explosion);
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
                                        Position = _ufo.Position,
                                        Scale = new Vector2(GameRoot.Scale), // TODO: Remove this scale hack! Make it configurable
                                        Sprite = _ufo._debri[rand.Next(_ufo._debri.Length)],
                                        TTL = 0.8f,
                                        Velocity = new Vector2(rand.Next(-100, 100), rand.Next(-100, 100))
                                    }))
                                .WithUpdate((rand, time, particle) =>
                                {
                                    particle.Position += particle.Velocity * time;
                                    particle.Angle += particle.AngularVelocity * time;
                                    particle.TTL -= time;
                                    particle.Color *= 0.99f;
                                })
                                .Build((int)DateTime.Now.Ticks, _ufo._draw, time => _ufo._state = new DeadState(_ufo));
            }

            public void Draw(float time)
            {
                _particleEngine.Draw(time);
            }

            public void Update(float time)
            {
                _particleEngine.Update(time);
            }
        }

        private sealed class DeadState : IState
        {
            public DeadState(Ufo ufo)
            {
                ufo._publisher.Publish(new UfoDestroyedEvent(ufo));
            }

            public void Draw(float time) { }

            public void Update(float time) { }
        }
    }

    public enum UfoState
    {
        Alive,
        Destroy,
        Dead
    }

    public sealed class UfoDestroyedEvent : IEvent
    {
        public UfoDestroyedEvent(Ufo ufo)
        {
            Id = Guid.NewGuid();
            Ufo = ufo;
        }

        public Guid Id { get; }

        public Ufo Ufo { get; }
    }
}
