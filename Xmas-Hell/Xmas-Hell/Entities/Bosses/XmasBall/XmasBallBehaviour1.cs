using System;
using Microsoft.Xna.Framework;
using Xmas_Hell.BulletML;

namespace Xmas_Hell.Entities.Bosses.XmasBall
{
    class XmasBallBehaviour1 : AbstractBossBehaviour
    {
        private TimeSpan _newPositionTime;
        private TimeSpan _bulletFrequence;

        public XmasBallBehaviour1(Boss boss) : base(boss)
        {
        }

        public override void Reset()
        {
            base.Reset();

            _newPositionTime = TimeSpan.Zero;
            _bulletFrequence = TimeSpan.Zero;

            Boss.CurrentAnimator.Play("Idle");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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
                Boss.AddBullet("sample", BulletType.Type1);
            }
        }
    }
}