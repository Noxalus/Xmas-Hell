using Microsoft.Xna.Framework;
using XmasHell.Physics;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;

namespace XmasHell.Entities.Bosses.XmasSnowman
{
    class Hat : ISpriterPhysicsEntity, IMovable
    {
        private readonly XmasSnowman _boss;
        private CollisionElement _boundingBox;
        private CustomSpriterAnimator _animator;
        private Vector2 _initialPosition;
        public bool Destroyed = false;
        private float _speed;

        public CustomSpriterAnimator GetCurrentAnimator()
        {
            return _animator;
        }

        public Vector2 Position()
        {
            return _animator.Position;
        }

        Vector2 IMovable.Position
        {
            get { return Position(); }
            set { Position(value); }
        }

        public void Position(Vector2 value)
        {
            _animator.Position = value;
        }

        public float Rotation()
        {
            return _animator.Rotation;
        }

        public void Rotation(float value)
        {
            _animator.Rotation = value;
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

        public Hat(XmasSnowman boss, CustomSpriterAnimator animator, Vector2 position)
        {
            _boss = boss;
            _animator = animator.Clone();
            _speed = 200;

            _initialPosition = position;

            Position(_initialPosition);

            // Physics
            _boundingBox = new SpriterCollisionCircle(this, "hat.png", Vector2.Zero, 0.7f);

            _boss.Game.GameManager.CollisionWorld.AddBossHitBox(_boundingBox);

            // Animations
            _animator.Play("Idle");

            ChangeHorizontalPosition();
        }

        public void Dispose()
        {
            _boss.Game.GameManager.CollisionWorld.RemoveBossBulletHitbox(_boundingBox);
            Destroyed = true;
        }

        private void ChangeHorizontalPosition()
        {
            this.CreateTweenChain(ChangeHorizontalPosition)
                .MoveTo(new Vector2(_boss.GetRandomHorizontalPosition(), _initialPosition.Y), 2f, EasingFunctions.ElasticInOut)
                ;
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
