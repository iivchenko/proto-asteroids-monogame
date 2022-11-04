using Core.Entities;
using Engine.Entities;
using Engine.Collisions;
using System;
using System.Linq;

namespace Core.Screens.GamePlay.Events
{
    public sealed class PlayersProjectileCollidesUfoEventHandler : BaseCollisionEventHandler<Projectile, Ufo>
    {
        private readonly GamePlayContext _context;
        private readonly GamePlayScoreManager _scores;
        private readonly IWorld _world;
        private readonly ICollisionService _collisionService;

        public PlayersProjectileCollidesUfoEventHandler(
            GamePlayContext context,
            GamePlayScoreManager score,
            IWorld world,
            ICollisionService collisionService)
        {
            _context = context;
            _scores = score;
            _world = world;
            _collisionService = collisionService;
        }

        protected override bool ExecuteConditionInternal(Projectile projectile, Ufo ufo)
            => projectile.Tags.Contains(GameTags.Player) && ufo.State == UfoState.Alive;

        protected override void ExecuteActionInternal(Projectile projectile, Ufo ufo)
        {
            _context.Scores += _scores.GetScore(ufo);
            _world.Remove(projectile);
            _collisionService.UnregisterBody(projectile);
            ufo.Destroy();
        }
    }
}