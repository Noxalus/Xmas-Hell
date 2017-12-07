using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using XmasHell.Physics;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;
using System;

namespace XmasHell.Entities.Bosses.XmasLog
{
    class Holly : AbstractEntity, ISpriterPhysicsEntity
    {
        private readonly XmasLog _boss;
        private CollisionElement _boundingBox;
        private CustomSpriterAnimator _animator;
        private float _angularSpeed;
        private List<CollisionElement> _boundingBoxes;
        private float _randomAnimationTime;
        private TimeSpan _expandTimer;

        public Vector2 Position()
        {
            return _animator.Position;
        }

        public override void Position(Vector2 position)
        {
            base.Position(position);
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
            _angularSpeed = 5f;

            var hollyLeafPosition = SpriterUtils.GetWorldPosition("holly-leaf.png", boss.CurrentAnimator);
            Position(hollyLeafPosition);
            _animator.Play("Growth");

            _animator.AnimationFinished += AnimationFinished;

            // Physics
            _boundingBoxes = new List<CollisionElement>
            {
                new SpriterCollisionCircle(this, "holly-leaf.png", new Vector2(-30, 0), 0.4f, "holly-leaf"),
                new SpriterCollisionCircle(this, "holly-leaf.png", new Vector2(45, 10), 0.3f, "holly-leaf"),
                new SpriterCollisionCircle(this, "holly-leaf.png", new Vector2(-30, 0), 0.4f, "holly-leaf_000"),
                new SpriterCollisionCircle(this, "holly-leaf.png", new Vector2(45, 10), 0.3f, "holly-leaf_000"),
                new SpriterCollisionCircle(this, "holly-leaf.png", new Vector2(-30, 0), 0.4f, "holly-leaf_001"),
                new SpriterCollisionCircle(this, "holly-leaf.png", new Vector2(45, 10), 0.3f, "holly-leaf_001"),
                new SpriterCollisionCircle(this, "holly-balls.png", Vector2.Zero, 0.8f)
            };

            foreach (var boundingBox in _boundingBoxes)
                _boss.Game.GameManager.CollisionWorld.AddBossHitBox(boundingBox);

            MoveTo(_boss.Game.ViewportAdapter.Center.ToVector2());
        }

        private void AnimationFinished(string animationName)
        {
            if (animationName == "Growth")
                Expand();
        }

        private void Expand()
        {
            if (_animator.CurrentAnimation.Name != "Expand")
                _animator.Play("Expand");

            var animationTime = (int)_animator.Length;

            _randomAnimationTime = _boss.Game.GameManager.Random.Next(animationTime / 10, animationTime);

            _animator.Speed = 1;
        }

        private void RevertExpand()
        {
            _animator.Speed = -1;
        }

        public void Dispose()
        {
            foreach (var boundingBox in _boundingBoxes)
                _boss.Game.GameManager.CollisionWorld.RemoveBossHitBox(boundingBox);

            _boundingBoxes.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_animator.CurrentAnimation.Name == "Expand")
            {
                if (_animator.Speed == 1)
                {
                    if ((_animator.Progress * _animator.Length) > _randomAnimationTime)
                    {
                        _animator.Speed = 0;
                        _expandTimer = TimeSpan.FromSeconds(_boss.Game.GameManager.Random.Next(1, 5));
                    }
                }
                else if (_animator.Speed == 0)
                {
                    _expandTimer -= gameTime.ElapsedGameTime;

                    if (_expandTimer.TotalSeconds < 0)
                        RevertExpand();
                }
                else if (_animator.Speed == -1)
                {
                    if (_animator.Progress == 0)
                        Expand();
                }
            }

            _animator.Rotation += _angularSpeed * gameTime.GetElapsedSeconds();
            _animator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animator.Draw(spriteBatch);
        }
    }
}
