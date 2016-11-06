using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RandomExtension;
using Xmas_Hell.BulletML;

namespace Xmas_Hell.Entities.Bosses.XmasBell
{
    class XmasBellBehaviour3 : AbstractBossBehaviour
    {
        private enum ScreenSide
        {
            Left, Top, Right, Bottom
        }

        private TimeSpan _bulletFrequence;

        public XmasBellBehaviour3(Boss boss) : base(boss)
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

            Boss.CurrentAnimator.Play("Troll");

            Boss.CurrentAnimator.AnimationFinished += delegate(string animationName)
            {
                if (animationName == "Troll")
                {
                    GetNewRandomPosition();
                    Boss.CurrentAnimator.Play("Troll");
                }
            };
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

            switch (side[randomSideIndex])
            {
                case ScreenSide.Left:
                    Boss.CurrentAnimator.Rotation = 0;
                    newXPosition = -Boss.Width() / 2f;
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

            // TODO: Will go from a side to another side of the screen
            //if (Boss.CurrentAnimator.Position.X > GameConfig.VirtualResolution.X + Boss.Width())
            //{
            //    GetNewYRandomPosition();
            //    Boss.Direction.X = -1;
            //}
            //else if (Boss.CurrentAnimator.Position.X < -Boss.Width())
            //{
            //    GetNewYRandomPosition();
            //    Boss.Direction.X = 1;
            //}

            //if (_bulletFrequence.TotalMilliseconds > 0)
            //    _bulletFrequence -= gameTime.ElapsedGameTime;
            //else
            //{
            //    _bulletFrequence = TimeSpan.FromSeconds(0.5f);
            //    Boss.TriggerPattern("XmasBell/pattern1", BulletType.Type2, false, Boss.ActionPointPosition());
            //}
        }
    }
}