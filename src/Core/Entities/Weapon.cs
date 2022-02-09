using Core.Events;
using Engine;
using Engine.Audio;
using Engine.Entities;
using Engine.Graphics;
using Engine.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Core.Entities
{
    public sealed class Weapon : IEntity<Guid>, IUpdatable
    {
        private readonly Vector2 _offset;
        private readonly TimeSpan _reload;
        private readonly IProjectileFactory _factory;
        private readonly IEventPublisher _eventService;
        private readonly IAudioPlayer _player;
        private readonly Random _random;

        private readonly Sprite _lazerSprite;
        private readonly Sound _lazer;

        private State _state;
        private double _reloading;

        public Weapon(
            Vector2 offset,
            TimeSpan reload,
            IProjectileFactory factory,
            IEventPublisher eventService,
            IAudioPlayer player,
            Sprite lazerSprite,
            Sound lazer)
        {
            _offset = offset;
            _reload = reload;
            _factory = factory;
            _eventService = eventService;
            _player = player;

            _lazerSprite = lazerSprite;
            _lazer = lazer;

            _state = State.Idle;
            _random = new Random();

            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
        public IEnumerable<string> Tags => Enumerable.Empty<string>();

        public void Update(float time)
        {
            switch(_state)
            {
                case State.Idle:
                    break;
                case State.Reload:
                    _reloading -= time;

                    if (_reloading <= 0)
                    {
                        _state = State.Idle;
                    }
                    break;
            }
        }

        public void Fire(Vector2 parentPosition, float parentRotation)
        {
            if (_state == State.Idle)
            {
                _state = State.Reload;
                _reloading = _reload.TotalSeconds;
                var position = Matrix2.CreateRotation(parentRotation) * _offset + parentPosition;
                var direction = parentRotation.ToDirection();
                var projectile = _factory.Create(position, direction, _lazerSprite);

                _eventService.Publish(new EntityCreatedEvent(projectile));

                var pitch = _random.Next(-50, 50) / 100.0f;
                _player.Play(_lazer, pitch);
            }
        }

        private enum State
        {
            Idle,
            Reload
        }
    }
}
