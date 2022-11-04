using Core.Entities;
using Engine.Graphics;
using Engine.Events;
using System.Numerics;

namespace Core.Screens.GamePlay.Events
{
    public static partial class EntitiesRules
    {
        public static partial class WhenPlayersShipDestroyed
        {
            public sealed class PlayersShipDestroyedEventHandler : IEventHandler<ShipDestroyedEvent>
            {
                private readonly IViewport _viewport;

                public PlayersShipDestroyedEventHandler(IViewport viewport)
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
