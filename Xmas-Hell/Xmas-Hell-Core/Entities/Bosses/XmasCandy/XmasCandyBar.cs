using Microsoft.Xna.Framework;
using XmasHell.Physics;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;
using XmasHell.Extensions;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandyBar : ISpriterPhysicsEntity
    {
        private readonly Boss _boss;
        private CollisionConvexPolygon _boundingBox;
        private CustomSpriterAnimator _animator;
        public bool Destroyed = false;

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

        public Vector2 Scale()
        {
            return _animator.Scale;
        }

        public void TakeDamage(float damage)
        {
            _boss.TakeDamage(damage * 0.25f);
        }

        public Vector2 ActionPointPosition()
        {
            var actionPointPosition = Vector2.Zero;

            foreach (var pointData in _animator.FrameData.PointData)
            {
                if (pointData.Key.StartsWith("action_point"))
                {
                    var actionPoint = new Vector2(pointData.Value.X, -pointData.Value.Y);
                    var rotatedActionPoint = MathExtension.RotatePoint(actionPoint, Rotation());

                    actionPointPosition = Position() + rotatedActionPoint;
                    break;
                }
            }

            return actionPointPosition;
        }

        public XmasCandyBar(Boss boss, CustomSpriterAnimator animator)
        {
            _boss = boss;
            _animator = animator;

            _animator.Progress = 0;
            _animator.Speed = 1;
            _animator.AnimationFinished += AnimationFinishedHandler;

            _boundingBox = new SpriterCollisionConvexPolygon(this, "body2.png");
            _boss.Game.GameManager.CollisionWorld.AddBossHitBox(_boundingBox);

            _animator.Play("StretchInBorderMoving");
        }

        public void Dispose()
        {
            _boss.Game.GameManager.CollisionWorld.RemoveBossHitBox(_boundingBox);
            _animator.AnimationFinished -= AnimationFinishedHandler;
            Destroyed = true;
        }

        public void StartStretchInAnimation()
        {
            _animator.Speed = 1;
            _animator.Play("StretchInBorderHeight");
        }

        private void AnimationFinishedHandler(string animationName)
        {
            if (animationName == "StretchInBorderWidth" || animationName == "StretchInBorderHeight")
                Dispose();
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
