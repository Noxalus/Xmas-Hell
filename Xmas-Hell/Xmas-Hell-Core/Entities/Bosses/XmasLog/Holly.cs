using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XmasHell.Physics;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;

namespace XmasHell.Entities.Bosses.XmasLog
{
    class Holly : ISpriterPhysicsEntity
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

        public Holly(XmasLog boss, CustomSpriterAnimator animator)
        {
            _boss = boss;
            _animator = animator.Clone();

            var hollyLeafPosition = SpriterUtils.GetWorldPosition("holly-leaf.png", boss.CurrentAnimator);
            Position(hollyLeafPosition);
            Position(new Vector2(GameConfig.VirtualResolution.X / 2f, GameConfig.VirtualResolution.Y / 2f));
            _animator.Play("Growth");
        }

        public void Dispose()
        {
        }

        public void Update(GameTime gameTime)
        {
            _animator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animator.Draw(spriteBatch);
        }
    }
}
