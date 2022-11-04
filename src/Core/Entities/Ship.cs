using Engine;
using Engine.Audio;
using Engine.Collisions;
using Engine.Entities;
using Engine.Graphics;
using Engine.Particles;
using Engine.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Core.Entities
{
    public sealed class Ship : IEntity<Guid>, IUpdatable, IDrawable, IBody
    {
        private readonly IPainter _draw;
        private readonly IEventPublisher _publisher;
        private readonly Sprite _sprite;
        private readonly Sprite[] _debri;
        private readonly Weapon _weapon;
        private readonly ShipTrail[] _trails;
        private readonly IAudioPlayer _audioPlayer;
        private readonly Sound _explosion;

        private readonly float _maxSpeed;
        private readonly float _maxAcceleration;
        private readonly float _maxRotation;
        private readonly float _maxAngularAcceleration;

        private Vector2 _velocity;
        private float _angularVelocity;
        private ShipAction _action;
        private IState _state;

        public Ship(
            IPainter draw,
            IEventPublisher publisher,
            Sprite sprite,
            Sprite[] debri,
            Weapon weapon,
            ShipTrail[] trails,
            IAudioPlayer audioPlayer,
            Sound explosion,
            float maxSpeed,
            float maxAcceleration,
            float maxRotation,
            float maxAngularAcceleration)
        {
            _draw = draw;
            _publisher = publisher;
            _sprite = sprite;
            _debri = debri;
            _weapon = weapon;
            _trails = trails;
            _audioPlayer = audioPlayer;
            _explosion = explosion;
            _maxSpeed = maxSpeed;
            _maxAcceleration = maxAcceleration;
            _maxRotation = maxRotation;
            _maxAngularAcceleration = maxAngularAcceleration;

            _velocity = Vector2.Zero;
            _action = ShipAction.None;

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
        public ShipState State 
        {
            get => _state switch
            {
                ResetState _ => ShipState.Reset,
                AliveState _ => ShipState.Alive,
                DestroyState _ => ShipState.Destroy,
                DeadState _ => ShipState.Dead,
                _ => throw new NotImplementedException()
            };
        }

        public void Apply(ShipAction action)
        {
            _action = action;
        }

        public void Reset()
        {
            _state = new ResetState(this);
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
            public readonly Ship _ship;

            public AliveState(Ship ship)
            {
                _ship = ship;
            }

            public virtual void Draw(float time)
            {
                _ship._trails.Iter(x => x.Draw());

                _ship
                    ._draw
                        .Draw(
                            _ship._sprite,
                            _ship.Position,
                            _ship.Origin,
                            _ship.Scale,
                            _ship.Rotation,
                            Colors.White);
            }

            public virtual void Update(float time)
            {
                _ship._weapon.Update(time);

                if (_ship._action.HasFlag(ShipAction.Left))
                {
                    var angularVelocity = _ship._angularVelocity - _ship._maxAngularAcceleration;
                    _ship._angularVelocity = Math.Abs(angularVelocity) > _ship._maxRotation ? -_ship._maxRotation : angularVelocity;

                    _ship.Rotation += _ship._angularVelocity * time;
                }
                else if (_ship._action.HasFlag(ShipAction.Right))
                {
                    var angularVelocity = _ship._angularVelocity + _ship._maxAngularAcceleration;
                    _ship._angularVelocity = Math.Abs(angularVelocity) > _ship._maxRotation ? _ship._maxRotation : angularVelocity;

                    _ship.Rotation += _ship._angularVelocity * time;
                }
                else
                {
                    _ship._angularVelocity = 0;
                }

                if (_ship._action.HasFlag(ShipAction.Accelerate))
                {
                    var velocity = _ship._velocity + _ship.Rotation.ToDirection() * _ship._maxAcceleration;

                    _ship._velocity = velocity.Length() > _ship._maxSpeed ? Vector2.Normalize(velocity) * _ship._maxSpeed : velocity;
                    _ship._trails.Iter(x => x.Update(true, _ship.Position, _ship.Rotation));
                }
                else
                {
                    _ship._trails.Iter(x => x.Update(false, _ship.Position, _ship.Rotation));
                }

                _ship.Position += _ship._velocity * time;

                if (_ship._action.HasFlag(ShipAction.Fire))
                    _ship._weapon.Fire(_ship.Position, _ship.Rotation);

                _ship._action = ShipAction.None;
            }
        }

        private sealed class DestroyState : IState
        {
            private readonly Ship _ship;
            private readonly ParticleEngine _particleEngine;

            public DestroyState(Ship ship)
            {
                _ship = ship;
                _ship._audioPlayer.Play(_ship._explosion);
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
                                        Position = _ship.Position,
                                        Scale = new Vector2(GameRoot.Scale), // TODO: Remove this scale hack! Make it configurable
                                        Sprite = _ship._debri[rand.Next(_ship._debri.Length)],
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
                                .Build((int)DateTime.Now.Ticks, _ship._draw, time => _ship._state = new DeadState(_ship));
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

        private sealed class ResetState : AliveState
        {
            private float _ttl = 2.5f;

            public ResetState(Ship ship)
                : base (ship)
            {
                _ship._velocity = Vector2.Zero;
            }

            public override void Draw(float time)
            {
                _ship._trails.Iter(x => x.Draw());

                var pulsate = MathF.Abs(MathF.Sin(_ttl * 15)) * 0.3f + 0.1f;

                _ship
                     ._draw
                         .Draw(
                             _ship._sprite,
                             _ship.Position,
                             _ship.Origin,
                             _ship.Scale,
                             _ship.Rotation,
                             Colors.White * pulsate);
            }

            public override void Update(float time)
            {
                base.Update(time);

                _ttl -= time;
                if (_ttl < 0)
                {
                    _ship._state = new AliveState(_ship);
                }
            }
        }

        private sealed class DeadState : IState
        {
            public DeadState(Ship ship) 
            {
                ship._publisher.Publish(new ShipDestroyedEvent(ship));
            }

            public void Draw(float time) { }

            public void Update(float time) { }
        }
    }

    public sealed class ShipTrail
    {
        private readonly Sprite _sprite;
        private readonly Vector2 _offset;
        private readonly Vector2 _origin;
        private readonly Vector2 _scale;
        private readonly IPainter _painer;

        private Vector2 _position;
        private float _rotation;

        private float _power;

        public ShipTrail(
            Sprite sprite, 
            Vector2 offset,
            Vector2 origin,
            Vector2 scale,
            IPainter painer)
        {
            _sprite = sprite;
            _offset = offset;
            _origin = origin;
            _scale = scale;
            _painer = painer;

            _power = 0;
        }

        public void Update(bool increase, Vector2 parentPosition, float parentRotation)
        {
            if (increase)
            {
                _power += 0.1f;
                _power = Math.Clamp(_power, 0, 1);

                if (_power == 1f) _power -= 0.7f;
            }
            else
            {
                _power -= 0.1f;
                _power = Math.Clamp(_power, 0, 1);
            }

            _position = Matrix2.CreateRotation(parentRotation) * _offset + parentPosition;
            _rotation = parentRotation;
        }

        public void Draw()
        {
            var rect = new Rectangle(0, 0, (int)_sprite.Width, (int)(_sprite.Height * _power));
            _painer.Draw(_sprite, _position, rect, _origin, _scale, _rotation, Colors.White);
        }
    }

    [Flags]
    public enum ShipAction
    {
        None = 0b0000,
        Accelerate = 0b0001,
        Left = 0b0010,
        Right = 0b0100,
        Fire = 0b1000
    }

    public enum ShipState
    {
        Alive,
        Destroy,
        Reset,
        Dead
    }

    public sealed class ShipDestroyedEvent : IEvent
    {
        public ShipDestroyedEvent(Ship ship)
        {
            Id = Guid.NewGuid();
            Ship = ship;
        }

        public Guid Id { get; }

        public Ship Ship { get; }
    }
}
