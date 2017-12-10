using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Timers;
using XmasHell.Physics;
using XmasHell.Physics.Collision;
using XmasHell.BulletML;
using XmasHell.Geometry;

namespace XmasHell.Entities.Bosses.XmasSnowflake
{
    class XmasSnowflakeBranch : AbstractDrawableEntity, IPhysicsEntity
    {
        private readonly Boss _boss;
        private Line _branchPlayerLine;
        private CollisionConvexPolygon _boundingBox;
        private CountdownTimer _canRushTimer;
        private CountdownTimer _rushTimer;
        private bool _rushing;
        private bool _canRush;
        private bool _destroyed;
        private float _angularSpeed;

        public float Rotation()
        {
            return Sprite.Rotation;
        }

        public Vector2 Origin()
        {
            return Vector2.Zero;
        }

        public Vector2 ScaleVector()
        {
            return Sprite.Scale;
        }

        public void TakeDamage(float damage)
        {
            _boss.TakeDamage(damage * 0.125f);
        }

        public bool Alive()
        {
            return !_destroyed;
        }

        public XmasSnowflakeBranch(Boss boss, Sprite sprite) : base(sprite)
        {
            Speed = 1500f;
            _boss = boss;

            _canRushTimer = new CountdownTimer(1f + (_boss.Game.GameManager.Random.NextDouble() * 3f));
            _rushTimer = new CountdownTimer(5f);
            _rushTimer.Stop();

            _rushing = false;
            _canRush = false;

            _angularSpeed = 2f + ((float)_boss.Game.GameManager.Random.NextDouble() * 3f);

            if (_boss.Game.GameManager.Random.NextDouble() > 0.5f)
                _angularSpeed *= -1;

            _destroyed = false;

            _canRushTimer.Completed += (sender, args) =>
            {
                _canRush = true;
                _rushTimer.Start();
            };

            _rushTimer.Completed += (sender, args) =>
            {
                if (!_destroyed && !TargetingPosition)
                {
                    RushToPlayer();
                }
            };

            Sprite.Origin = new Vector2(0.5f * Sprite.TextureRegion.Width, 0.5f * Sprite.TextureRegion.Height);

            // Physics
            var bbWidth = sprite.TextureRegion.Width / 4f;
            var bbLocalPosition = new Vector2((sprite.TextureRegion.Width / 2f) - (bbWidth / 2f), 0f);
            var startX = -sprite.TextureRegion.Width / 2f;
            var startY = -sprite.TextureRegion.Height / 2f;
            var vertices = new List<Vector2>()
            {
                new Vector2(startX, startY),
                new Vector2(startX + bbWidth, startY),
                new Vector2(startX + bbWidth, startY + sprite.TextureRegion.Height),
                new Vector2(startX, startY + sprite.TextureRegion.Height)
            };

            _boundingBox = new CollisionConvexPolygon(this, bbLocalPosition, vertices);
            _boss.AddHitBox(_boundingBox);

            _branchPlayerLine = new Line(Position(), _boss.GetPlayerPosition());

        }

        public void Dispose()
        {
            _boss.Game.GameManager.CollisionWorld.RemoveBossHitBox(_boundingBox);
        }

        private void Destroy()
        {
            // Shake the camera + trigger a simple bullet pattern
            _boss.Game.Camera.Shake(1f, 10f);
            _boss.Game.GameManager.MoverManager.TriggerPattern("XmasSnowflake/pattern2", BulletType.Type3, false, Position());

            _destroyed = true;
        }

        private void RushToPlayer()
        {
            Sprite.Rotation = Vector2.Normalize(_boss.GetPlayerPosition() - Position()).ToAngle() + (float)Math.PI;

            var branchPlayerLine = new Line(Position(), _boss.GetPlayerPosition());
            var newPosition = Vector2.Zero;
            if (_boss.GetLineWallIntersectionPosition(branchPlayerLine, ref newPosition))
            {
                MoveTo(newPosition, true);
                _rushing = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            _canRushTimer.Update(gameTime);
            _rushTimer.Update(gameTime);

            if (!TargetingPosition && !_rushing)
            {
                // Follow the player
                Sprite.Rotation += _angularSpeed * dt;

                if (_canRush)
                {
                    var playerAngle = Vector2.Normalize(_boss.GetPlayerPosition() - Position()).ToAngle() + (float) Math.PI;
                    if (Math.Abs(MathHelper.WrapAngle(Sprite.Rotation) - playerAngle) < 0.1f)
                    {
                        _canRush = false;
                        RushToPlayer();
                    }
                }
            }

            if (_rushing && Position() == TargetPosition)
            {
                _rushing = false;

                //_canRushTimer.Interval = TimeSpan.FromSeconds(1 + (_boss.Game.GameManager.Random.NextDouble() * 5));
                //_canRushTimer.Restart();
                //Speed *= 1.2f;

                Destroy();
            }
        }
    }
}
