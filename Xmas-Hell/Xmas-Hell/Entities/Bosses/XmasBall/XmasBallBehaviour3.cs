using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Xmas_Hell.BulletML;
using Xmas_Hell.Geometry;

namespace Xmas_Hell.Entities.Bosses.XmasBall
{
    class XmasBallBehaviour3 : AbstractBossBehaviour
    {
        private Line _bossPlayerLine;
        private readonly Line _leftWallLine;
        private readonly Line _bottomWallLine;
        private readonly Line _upWallLine;
        private readonly Line _rightWallLine;
        private Vector2 _newPosition;
        private TimeSpan _stunnedTimer;
        private bool _stunned;
        private bool _lockingTarget;
        private TimeSpan _lockingTargetTimer;
        private bool _charging;

        public XmasBallBehaviour3(Boss boss) : base(boss)
        {
            _bottomWallLine = new Line(
                new Vector2(0f, GameConfig.VirtualResolution.Y),
                new Vector2(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y)
            );

            _leftWallLine = new Line(
                new Vector2(0f, 0f),
                new Vector2(0f, GameConfig.VirtualResolution.Y)
            );

            _rightWallLine = new Line(
                new Vector2(GameConfig.VirtualResolution.X, 0f),
                new Vector2(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y)
            );

            _upWallLine = new Line(
                new Vector2(0f, 0f),
                new Vector2(GameConfig.VirtualResolution.X, 0f)
            );
        }

        public override void Start()
        {
            base.Start();

            _newPosition = Vector2.Zero;
            Boss.Speed = 100f;
            _charging = false;
            _stunned = false;

            // Start to lock the player
            _lockingTarget = true;
            _lockingTargetTimer = TimeSpan.Zero;
        }

        public override void Stop()
        {
            base.Stop();

            Boss.Acceleration = Vector2.One;
            Boss.CurrentAnimator.Rotation = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var currentPosition = Boss.CurrentAnimator.Position;

            if (_stunned)
            {
                if (Boss.CurrentAnimator.CurrentAnimation.Name != "Stunned")
                    Boss.CurrentAnimator.Transition("Stunned", 0.5f);

                if (_stunnedTimer.TotalMilliseconds <= 0)
                {
                    _stunned = false;
                    _stunnedTimer = TimeSpan.Zero;

                    _lockingTarget = true;
                    _lockingTargetTimer = TimeSpan.FromSeconds(
                        RandomExtension.RandomExtension.NextDouble(
                            Boss.Game.GameManager.Random,
                            0.5f, 1.5f
                        )
                    );
                }
                else
                    _stunnedTimer -= gameTime.ElapsedGameTime;
            }

            if (_lockingTarget && !_stunned)
            {
                if (Boss.CurrentAnimator.CurrentAnimation.Name != "No_Animation")
                    Boss.CurrentAnimator.Play("No_Animation");

                Boss.RotateTo(Boss.GetPlayerDirection().ToAngle(), true);

                if (_lockingTargetTimer.TotalMilliseconds <= 0)
                {
                    _lockingTarget = false;
                    _lockingTargetTimer = TimeSpan.Zero;

                    _charging = true;
                }
                else
                {
                    Boss.RotateTo(Boss.GetPlayerDirection().ToAngle(), true);
                    _lockingTargetTimer -= gameTime.ElapsedGameTime;
                }
            }

            if (!Boss.TargetingPosition && !Boss.TargetingAngle && _charging)
            {
                _charging = false;
                Boss.CurrentAnimator.Transition("Idle", 0.5f);

                var playerDirection = Boss.GetPlayerDirection();
                var maxDistance = (float) Math.Sqrt(
                    GameConfig.VirtualResolution.X*GameConfig.VirtualResolution.X +
                    GameConfig.VirtualResolution.Y*GameConfig.VirtualResolution.Y
                );

                Boss.CurrentAnimator.Rotation = playerDirection.ToAngle();

                var fartherPlayerPosition = currentPosition + (playerDirection*maxDistance);

                _bossPlayerLine = new Line(
                    currentPosition,
                    fartherPlayerPosition
                );

                _newPosition = Vector2.Zero;

                var hasNewPosition =
                    MathHelperExtension.LinesIntersect(_bottomWallLine, _bossPlayerLine, ref _newPosition) ||
                    MathHelperExtension.LinesIntersect(_leftWallLine, _bossPlayerLine, ref _newPosition) ||
                    MathHelperExtension.LinesIntersect(_rightWallLine, _bossPlayerLine, ref _newPosition) ||
                    MathHelperExtension.LinesIntersect(_upWallLine, _bossPlayerLine, ref _newPosition);

                if (hasNewPosition)
                    Boss.MoveTo(_newPosition);
            }

            if (Boss.TargetingPosition)
            {
                Boss.Acceleration += new Vector2(0.5f);

                if (currentPosition.X < Boss.Width() / 2f ||
                    currentPosition.X > GameConfig.VirtualResolution.X - Boss.Width() / 2f ||
                    currentPosition.Y < Boss.Height() / 2f ||
                    currentPosition.Y > GameConfig.VirtualResolution.Y - Boss.Height() / 2f)
                {
                    Boss.Acceleration = Vector2.One;
                    Boss.TargetingPosition = false;
                    Boss.Game.Camera.Shake(0.5f, 50f);
                    Boss.CurrentAnimator.Transition("Stunned", 0.5f);

                    var patternPosition = currentPosition;

                    if (currentPosition.X < Boss.Width() / 2f)
                        patternPosition.X -= Boss.Width() / 2f;
                    else if (currentPosition.X > GameConfig.VirtualResolution.X - Boss.Width() / 2f)
                        patternPosition.X += Boss.Width() / 2f;
                    else if (currentPosition.Y < Boss.Height() / 2f)
                        patternPosition.Y -= Boss.Height() / 2f;
                    else if (currentPosition.Y > GameConfig.VirtualResolution.Y - Boss.Height() / 2f)
                        patternPosition.Y += Boss.Height() / 2f;

                    Boss.TriggerPattern("XmasBall/pattern3", BulletType.Type3, false, patternPosition);

                    Boss.CurrentAnimator.Position = new Vector2(
                        MathHelper.Clamp(Boss.CurrentAnimator.Position.X, Boss.Width() / 2f,
                            GameConfig.VirtualResolution.X - Boss.Width() / 2f),
                        MathHelper.Clamp(Boss.CurrentAnimator.Position.Y, Boss.Height() / 2f,
                            GameConfig.VirtualResolution.Y - Boss.Height() / 2f)
                    );

                    _stunnedTimer = TimeSpan.FromSeconds(
                        RandomExtension.RandomExtension.NextDouble(
                            Boss.Game.GameManager.Random,
                            1, 2
                        )
                    );
                    _stunned = true;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.DrawLine(_bossPlayerLine.First, _bossPlayerLine.Second, Color.Red, 5f);
            //spriteBatch.DrawLine(_leftWallLine.First, _leftWallLine.Second, Color.White, 5f);
            //spriteBatch.DrawLine(_bottomWallLine.First, _bottomWallLine.Second, Color.Yellow, 5f);
            //spriteBatch.DrawLine(_rightWallLine.First, _rightWallLine.Second, Color.Brown, 5f);
            //spriteBatch.DrawLine(_upWallLine.First, _upWallLine.Second, Color.Green, 5f);
        }
    }
}