using System;
using Microsoft.Xna.Framework;

namespace Xmas_Hell.Entities.Bosses.XmasBall
{
    class XmasBallBehaviour1 : AbstractBossBehaviour
    {
        private TimeSpan _newPositionTime;
        private TimeSpan _bulletFrequence;

        public XmasBallBehaviour1(Boss boss) : base(boss)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (Boss.CurrentAnimator.Position.X > GameConfig.VirtualResolution.X - (Boss.Width() / 2f))
                Boss.Direction = -(float)Math.PI;
            else if (Boss.CurrentAnimator.Position.X < Boss.Width() / 2f)
                Boss.Direction = 0f;

            // TODO: Add this to an extension of the MathHelper
            var direction = new Vector2(
                (float)Math.Cos(Boss.Direction),
                -(float)Math.Sin(Boss.Direction)
            );

            //CurrentAnimator.Position += new Vector2(Speed * gameTime.GetElapsedSeconds(), 0f) * direction;

            if (_newPositionTime.TotalMilliseconds > 0)
            {
                if (!Boss.TargetingPosition)
                    _newPositionTime -= gameTime.ElapsedGameTime;
            }
            else
            {
                _newPositionTime = TimeSpan.FromSeconds((Boss.Game.GameManager.Random.NextDouble() * 2.5) + 0.5);

                var newPosition = new Vector2(
                    Boss.Game.GameManager.Random.Next((int)(Boss.Width() / 2f), GameConfig.VirtualResolution.X - (int)(Boss.Width() / 2f)),
                    Boss.Game.GameManager.Random.Next((int)(Boss.Height() / 2f) + 42, 288 - (int)(Boss.Height() / 2f))
                );

                Boss.MoveTo(newPosition);
            }

            if (_bulletFrequence.TotalMilliseconds > 0)
                _bulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                _bulletFrequence = TimeSpan.FromTicks(GameConfig.PlayerShootFrequency.Ticks);
                Boss.AddBullet();
            }
        }
    }
}