using Microsoft.Xna.Framework;
using XmasHell.Physics;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;

namespace XmasHell.Entities.Bosses.XmasLog
{
    class BabyLog : ISpriterPhysicsEntity
    {
        private readonly XmasLog _boss;
        private CollisionElement _boundingBox;
        private CustomSpriterAnimator _animator;

        public Vector2 Position()
        {
            return _animator.Position;
        }

        public void Position(Vector2 position)
        {
            _animator.Position = position;
        }

        public float Rotation()
        {
            return _animator.Rotation;
        }

        public Vector2 ScaleVector()
        {
            return _animator.Scale;
        }

        public Vector2 Origin()
        {
            return _animator.Origin();
        }

        public void TakeDamage(float damage)
        {
        }

        public CustomSpriterAnimator GetCurrentAnimator()
        {
            return _animator;
        }

        public BabyLog(XmasLog boss, CustomSpriterAnimator animator, Vector2 position)
        {
            _boss = boss;
            _animator = animator.Clone();
            Position(position);
        }

        public void Update(GameTime gameTime)
        {
            _animator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }
    }
}
