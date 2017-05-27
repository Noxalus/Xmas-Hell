using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XmasHell.Extensions;
using XmasHell.BulletML;

namespace XmasHell.Entities.Bosses.XmasBell
{
    class XmasBellBehaviour4 : AbstractBossBehaviour
    {
        private TimeSpan _bulletFrequence;
        private bool _centerPattern;
        private TimeSpan _centerPatternDuration;

        public XmasBellBehaviour4(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            _bulletFrequence = TimeSpan.Zero;
            _centerPattern = false;
            Boss.CurrentAnimator.Rotation = 0;
            Boss.CurrentAnimator.Speed = 1f;

            Boss.CurrentAnimator.AnimationFinished += delegate (string animationName)
            {
                if (animationName == "Troll" || animationName == "Troll2" || animationName == "Troll3")
                {
                    GetNewRandomPosition();
                    PlayRandomTrollAnimation();
                }
            };

            Boss.CurrentAnimator.EventTriggered += delegate (string eventName)
            {
                if (eventName == "shoot")
                {
                    Boss.Game.GameManager.MoverManager.TriggerPattern("XmasBell/pattern4", BulletType.Type2, false, Boss.ActionPointPosition(), Boss.ActionPointDirection());
                }
            };

            GetNewRandomPosition();
            PlayRandomTrollAnimation();
        }

        private void PlayRandomTrollAnimation()
        {
            var randomNumber = Boss.Game.GameManager.Random.NextDouble();

            randomNumber = 0.74f;

            if (randomNumber < 0.25f)
            {
                Boss.CurrentAnimator.Play("Troll");
            }
            else if(randomNumber < 0.5f)
            {
                Boss.CurrentAnimator.Play("Troll2");
            }
            else if (randomNumber < 0.75f)
            {
                Boss.CurrentAnimator.Play("Troll3");
            }
            else
            {
                StartCenterPattern();
            }
        }

        public override void Stop()
        {
            base.Stop();

            Boss.CurrentAnimator.Rotation = 0;
        }

        private void GetNewRandomPosition()
        {
            var side = new List<ScreenSide>()
            {
              ScreenSide.Left, ScreenSide.Top, ScreenSide.Right, ScreenSide.Bottom
            };

            var randomSideIndex = Boss.Game.GameManager.Random.Next(side.Count);
            //randomSideIndex = 2;

            float newXPosition;
            float newYPosition;

            switch (side[randomSideIndex])
            {
                case ScreenSide.Left:
                    Boss.CurrentAnimator.Rotation = 0;
                    newXPosition = -Boss.Width() / 2;
                    newYPosition = Boss.Game.GameManager.Random.NextFloat(
                        Boss.Height() / 2f,
                        GameConfig.VirtualResolution.Y - Boss.Height() / 2f
                    );
                    break;
                case ScreenSide.Top:
                    Boss.CurrentAnimator.Rotation = MathHelper.ToRadians(90f);
                    newXPosition = Boss.Game.GameManager.Random.NextFloat(
                        Boss.Width() / 2f,
                        GameConfig.VirtualResolution.X - Boss.Width() / 2f
                    );
                    newYPosition = -Boss.Width() / 2f;
                    break;
                case ScreenSide.Right:
                    Boss.CurrentAnimator.Rotation = MathHelper.ToRadians(180f);
                    newXPosition = GameConfig.VirtualResolution.X + (Boss.Width() / 2f);
                    newYPosition = Boss.Game.GameManager.Random.NextFloat(
                        Boss.Height() / 2f,
                        GameConfig.VirtualResolution.Y - Boss.Height() / 2f
                    );
                    break;
                case ScreenSide.Bottom:
                    Boss.CurrentAnimator.Rotation = MathHelper.ToRadians(-90f);
                    newXPosition = Boss.Game.GameManager.Random.NextFloat(
                        Boss.Width() / 2f,
                        GameConfig.VirtualResolution.X - Boss.Width() / 2f
                    );
                    newYPosition = GameConfig.VirtualResolution.Y + (Boss.Width() / 2f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Boss.CurrentAnimator.Position = new Vector2(newXPosition, newYPosition);
        }

        private void StartCenterPattern()
        {
            Boss.CurrentAnimator.Play("Idle");
            Boss.CurrentAnimator.Rotation = 0;
            Boss.Invincible = true;

            _centerPattern = true;
            _centerPatternDuration = TimeSpan.FromSeconds(Boss.Game.GameManager.Random.Next(10, 30));
            Boss.MoveToCenter();
            Boss.ShootTimerTime = 1;
            Boss.ShootTimerFinished += delegate(object sender, float e)
            {
                Boss.Game.GameManager.MoverManager.TriggerPattern("XmasBell/pattern2", BulletType.Type2, false, Boss.ActionPointPosition());
            };
        }

        private void StopCenterPattern()
        {
            _centerPattern = false;
            var randomPositionBounds = new Rectangle(
                Boss.Game.ViewportAdapter.VirtualWidth / 2,
                (int)(Boss.Game.ViewportAdapter.VirtualHeight / 1.25),
                (int)(Boss.Game.ViewportAdapter.VirtualWidth / 1.5),
                (int)(Boss.Game.ViewportAdapter.VirtualHeight)
            );

            //Boss.Position(Boss.Game.GameManager.GetRandomPosition(false));
            //Boss.MoveOutside();
            PlayRandomTrollAnimation();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Increase animation speed ovdr time
            float delaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Boss.CurrentAnimator.Speed = MathHelper.Clamp(Boss.CurrentAnimator.Speed + 0.05f * delaTime, 1f, 4f);

            if (Xmas_Hell_Core.Controls.InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                StopCenterPattern();
            }

            if (_centerPattern)
            {
                var diff = Boss.Position() - Boss.Game.ViewportAdapter.Center.ToVector2();
                if (Math.Abs(diff.X) < 0.01f && Math.Abs(diff.Y) < 0.01f)
                {
                    if (_centerPatternDuration.TotalMilliseconds > 0)
                    {
                        _centerPatternDuration -= gameTime.ElapsedGameTime;
                    }
                    else
                    {
                        StopCenterPattern();
                    }
                }
            }
        }
    }
}