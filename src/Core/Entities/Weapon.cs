using Core.Events;
using Engine;
using Engine.Audio;
using Engine.Entities;
using Engine.Graphics;
using Engine.Events;
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
        private readonly string _tag;

        private readonly Sprite _lazerSprite;
        private readonly Sound _lazer;

        private WeaponState _state;
        private double _reloading;

        public Weapon(
            Vector2 offset,
            TimeSpan reload,
            IProjectileFactory factory,
            IEventPublisher eventService,
            IAudioPlayer player,
            Sprite lazerSprite,
            Sound lazer,
            WeaponState initialState,
            string tag)
        {
            _offset = offset;
            _reload = reload;
            _factory = factory;
            _eventService = eventService;
            _player = player;
            _lazerSprite = lazerSprite;
            _lazer = lazer;
            _state = initialState;
            _tag = tag;

            switch(initialState)
            {
                case WeaponState.Idle:
                    _reloading = 0.0;
                    break;

                case WeaponState.Reload:
                    _reloading = reload.TotalSeconds;
                    break;
            }

            _random = new Random();

            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
        public IEnumerable<string> Tags => Enumerable.Empty<string>();
        public WeaponState State { get => _state; }

        public void Update(float time)
        {
            switch(_state)
            {
                case WeaponState.Idle:
                    break;
                case WeaponState.Reload:
                    _reloading -= time;

                    if (_reloading <= 0)
                    {
                        _state = WeaponState.Idle;
                    }
                    break;
            }
        }

        public void Fire(Vector2 parentPosition, float parentRotation)
        {
            if (_state == WeaponState.Idle)
            {
                _state = WeaponState.Reload;
                _reloading = _reload.TotalSeconds;
                var position = Matrix2.CreateRotation(parentRotation) * _offset + parentPosition;
                var direction = parentRotation.ToDirection();
                var projectile = _factory.Create(position, direction, _lazerSprite, _tag);

                _eventService.Publish(new EntityCreatedEvent(projectile));

                var pitch = _random.Next(-50, 50) / 100.0f;
                _player.Play(_lazer, pitch);
            }
        }
    }

    public enum WeaponState
    {
        Idle,
        Reload
    }
}
