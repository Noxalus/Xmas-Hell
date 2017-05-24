using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XmasHell.Extensions;
using XmasHell.BulletML;

namespace XmasHell.Entities.Bosses.XmasBell
{
    class XmasBellBehaviour4 : AbstractBossBehaviour
    {
        private enum ScreenSide
        {
            Left, Top, Right, Bottom
        }

        private TimeSpan _bulletFrequence;

        public XmasBellBehaviour4(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.Speed = 500f;
            _bulletFrequence = TimeSpan.Zero;

            Boss.CurrentAnimator.Position = new Vector2(
                -(int)(Boss.Width() / 2f),
                Boss.Game.GameManager.Random.Next((int)(Boss.Height() / 2f), GameConfig.VirtualResolution.Y - (int)(Boss.Height() / 2f))
            );


            //Boss.CurrentAnimator.Position = new Vector2(
            //    GameConfig.VirtualResolution.X / 2f - (int)(Boss.Width() / 4f),
            //    GameConfig.VirtualResolution.Y / 2f - (int)(Boss.Height() / 4f)
            //);

            Boss.CurrentAnimator.Play("Troll3");

            Boss.CurrentAnimator.AnimationFinished += delegate (string animationName)
            {
                if (animationName == "Troll3")
                {
                    GetNewRandomPosition();
                    Boss.CurrentAnimator.Play("Troll3");
                }
            };

            Boss.CurrentAnimator.EventTriggered += AnimationEventTriggered;
        }

        private void AnimationEventTriggered(string eventName)
        {
            if (eventName == "shoot")
            {
                Boss.Game.GameManager.MoverManager.TriggerPattern("XmasBell/pattern2", BulletType.Type2, false, Boss.ActionPointPosition());
            }
        }

        public override void Stop()
        {
            base.Stop();
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

            //randomSideIndex = 0;

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
                    newYPosition = -Boss.Height() / 2f;
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}