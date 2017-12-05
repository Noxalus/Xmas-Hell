using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
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
        private float _angularSpeed;
        private List<CollisionElement> _boundingBoxes;

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
            _angularSpeed = 20f;

            var hollyLeafPosition = SpriterUtils.GetWorldPosition("holly-leaf.png", boss.CurrentAnimator);
            Position(hollyLeafPosition);
            _animator.Play("Growth");
            _animator.Play("Idle");

            _animator.AnimationFinished += AnimationFinished;

            // Physics
            _boundingBoxes = new List<CollisionElement>
            {
                new SpriterCollisionCircle(this, "holly-leaf.png", Vector2.Zero, 0.6f),
                new SpriterCollisionCircle(this, "holly-balls.png")
            };

            foreach (var boundingBox in _boundingBoxes)
                _boss.Game.GameManager.CollisionWorld.AddBossHitBox(boundingBox);
        }

        private void AnimationFinished(string animationName)
        {
            if (animationName == "Growth")
            {
                _animator.Play("Expand");
            }
        }

        public void Dispose()
        {
            foreach (var boundingBox in _boundingBoxes)
                _boss.Game.GameManager.CollisionWorld.RemoveBossHitBox(boundingBox);

            _boundingBoxes.Clear();
        }

        public void Update(GameTime gameTime)
        {
            _animator.Rotation += _angularSpeed * gameTime.GetElapsedSeconds();
            _animator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animator.Draw(spriteBatch);
        }
    }
}
