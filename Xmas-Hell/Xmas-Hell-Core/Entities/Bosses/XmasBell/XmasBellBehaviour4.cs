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
        private bool _trollPattern;
        private bool _centerPattern;
        private TimeSpan _trollPatternDuration;
        private TimeSpan _centerPatternDuration;

        public XmasBellBehaviour4(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            _bulletFrequence = TimeSpan.Zero;
            _trollPattern = false;
            _centerPattern = false;
            Boss.Speed = 500f;
            Boss.CurrentAnimator.Rotation = 0;
            Boss.CurrentAnimator.Speed = 1f;

            // Events
            Boss.CurrentAnimator.AnimationFinished += AnimationFinished;
            Boss.CurrentAnimator.EventTriggered += AnimationEventTriggered;

            Boss.MoveOutside();
        }

        private void AnimationFinished(string animationName)
        {
            if (animationName.StartsWith("Troll"))
            {
                GetNewRandomPosition();
                PlayRandomTrollAnimation();
            }
        }

        private void AnimationEventTriggered(string eventName)
        {
            if (eventName == "shoot")
            {
                Boss.Game.GameManager.MoverManager.TriggerPattern(
                    "XmasBell/pattern4", BulletType.Type2, false, Boss.ActionPointPosition(), Boss.ActionPointDirection()
                );
            }
        }

        private void PlayRandomTrollAnimation()
        {
            var randomNumber = Boss.Game.GameManager.Random.Next(3);

            if (randomNumber == 0)
                Boss.CurrentAnimator.Play("Troll");
            else if(randomNumber == 1)
                Boss.CurrentAnimator.Play("Troll2");
            else
                Boss.CurrentAnimator.Play("Troll3");
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

        private void StartTrollPattern()
        {
            _trollPattern = true;
            _trollPatternDuration = TimeSpan.FromSeconds(Boss.Game.GameManager.Random.Next(10, 30));
            GetNewRandomPosition();
            PlayRandomTrollAnimation();
        }

        private void StopTrollPattern()
        {
            _trollPattern = false;
            StartCenterPattern();
        }

        private void StartCenterPattern()
        {
            Boss.CurrentAnimator.Play("Idle");
            Boss.CurrentAnimator.Rotation = 0;
            Boss.Invincible = true;

            _centerPattern = true;
            _centerPatternDuration = TimeSpan.FromSeconds(Boss.Game.GameManager.Random.Next(5, 10));
            Boss.MoveToCenter();

            Boss.ShootTimerTime = 0.2f;
            Boss.ShootTimerFinished += ShootTimerFinished;
        }

        private void ShootTimerFinished(object sender, float e)
        {
            Boss.Game.GameManager.MoverManager.TriggerPattern("XmasBell/pattern4", BulletType.Type2, false, Boss.ActionPointPosition(), Boss.ActionPointDirection());
        }

        private void StopCenterPattern()
        {
            _centerPattern = false;
            Boss.StartShootTimer = false;
            Boss.Invincible = false;
            Boss.ShootTimerFinished -= ShootTimerFinished;
            Boss.Rotation(0);
            Boss.CurrentAnimator.Play("Idle");
            Boss.MoveOutside();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!_centerPattern && !Boss.TargetingPosition && Boss.IsOutside && Boss.CurrentAnimator.CurrentAnimation.Name == "Idle")
                StartTrollPattern();

            // Increase animation speed over time
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Boss.CurrentAnimator.Speed = MathHelper.Clamp(Boss.CurrentAnimator.Speed + 0.01f * deltaTime, 1f, 4f);

            UpdateTrollPattern(gameTime);
            UpdateCenterPattern(gameTime);
        }

        private void UpdateTrollPattern(GameTime gameTime)
        {
            if (!_trollPattern)
                return;

            if (_trollPatternDuration.TotalMilliseconds > 0)
                _trollPatternDuration -= gameTime.ElapsedGameTime;
            else
                StopTrollPattern();
        }

        private void UpdateCenterPattern(GameTime gameTime)
        {
            if (!_centerPattern)
                return;

            if (Boss.Position() == Boss.Game.ViewportAdapter.Center.ToVector2())
            {
                Boss.StartShootTimer = true;
                Boss.CurrentAnimator.Play("No_Animation");

                if (_centerPatternDuration.TotalMilliseconds > 0)
                {
                    _centerPatternDuration -= gameTime.ElapsedGameTime;

                    float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Boss.Rotation(Boss.Rotation() + (2.5f * deltaTime));
                }
                else
                    StopCenterPattern();
            }
        }
    }
}