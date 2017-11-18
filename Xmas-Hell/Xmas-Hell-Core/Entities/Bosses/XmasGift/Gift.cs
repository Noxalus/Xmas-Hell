using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using XmasHell.Physics;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;
using XmasHell.Extensions;
using Microsoft.Xna.Framework.Graphics;

namespace XmasHell.Entities.Bosses.XmasGift
{
    class Gift : ISpriterPhysicsEntity
    {
        private readonly XmasGift _boss;
        private CollisionElement _boundingBox;
        private CustomSpriterAnimator _animator;
        public bool Destroyed = false;
        private Body _body;

        public CustomSpriterAnimator GetCurrentAnimator()
        {
            return _animator;
        }

        public Vector2 Position()
        {
            return _animator.Position;
        }

        public float Rotation()
        {
            return _animator.Rotation;
        }

        public Vector2 Origin()
        {
            return _animator.Origin();
        }

        public Vector2 ScaleVector()
        {
            return _animator.Scale;
        }

        public void TakeDamage(float damage)
        {
            _boss.TakeDamage(damage * 0.25f);
        }

        public Gift(XmasGift boss, CustomSpriterAnimator animator)
        {
            _boss = boss;
            _animator = animator.Clone();

            // Physics
            _boundingBox = _boss.CreateBoundingBox(this);
            _boss.Game.GameManager.CollisionWorld.AddBossHitBox(_boundingBox);

            var randomSpawnBounds = new Rectangle(
                100, 0,
                GameConfig.VirtualResolution.X - 100, 300
            );

            _body = _boss.CreateGiftBody(_boss.Game.GameManager.GetRandomPosition(false, randomSpawnBounds));

            _animator.Play("NoAnimation");
        }

        public void Dispose()
        {
            _boss.Game.GameManager.CollisionWorld.RemoveBossHitBox(_boundingBox);
            Destroyed = true;
        }

        public void Update(GameTime gameTime)
        {
            _animator.Position = ConvertUnits.ToDisplayUnits(_body.Position);
            _animator.Rotation = _body.Rotation;

            _animator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animator.Draw(spriteBatch);
        }
    }
}
