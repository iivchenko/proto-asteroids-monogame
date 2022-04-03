using Core.Entities;
using Core.Events;
using Engine.Collisions;
using Engine.Entities;
using Engine.Graphics;
using Engine.Rules;
using System.Numerics;

namespace Core.Screens.GamePlay.Rules
{
    public static class EntitiesRules
    {
        public static class WhenEntityIsCreated
        {
            public sealed class ThenAddItToTheWorld : IRule<EntityCreatedEvent>
            {
                private readonly IWorld _world;

                public ThenAddItToTheWorld(IWorld world)
                {
                    _world = world;
                }

                public bool ExecuteCondition(EntityCreatedEvent @event) => true;

                public void ExecuteAction(EntityCreatedEvent @event)
                {
                    _world.Add(@event.Entity);
                }
            }
        }
       
        public static class WhenAsteoidDestroyed
        {
            public sealed class ThenRemoveAsteroid : IRule<AsteroidDestroyedEvent>
            {
                private readonly IWorld _world;
                private readonly ICollisionService _collisionService;

                public ThenRemoveAsteroid(IWorld world, ICollisionService collisionService)
                {
                    _world = world;
                    _collisionService = collisionService;
                }

                public bool ExecuteCondition(AsteroidDestroyedEvent @event) => true;

                public void ExecuteAction(AsteroidDestroyedEvent @event)
                {
                    _world.Remove(@event.Asteroid);
                    _collisionService.UnregisterBody(@event.Asteroid);
                }
            }
        }

        public static class WhenPlayersShipDestroyed
        {
            public sealed class ThenResetPlayersShip : IRule<ShipDestroyedEvent>
            {
                private readonly IViewport _viewport;

                public ThenResetPlayersShip(IViewport viewport)
                {
                    _viewport = viewport;
                }

                public bool ExecuteCondition(ShipDestroyedEvent @event) => true;

                public void ExecuteAction(ShipDestroyedEvent @event)
                {
                    @event.Ship.Position = new Vector2(_viewport.Width / 2, _viewport.Height / 2);
                    @event.Ship.Reset();
                }
            }
        }
    }
}
