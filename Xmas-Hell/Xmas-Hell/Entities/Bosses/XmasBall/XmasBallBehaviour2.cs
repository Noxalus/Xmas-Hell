using System;
using Microsoft.Xna.Framework;

namespace Xmas_Hell.Entities.Bosses.XmasBall
{
    class XmasBallBehaviour2 : AbstractBossBehaviour
    {
        private Vector2 _screenCenter;
        private TimeSpan _bulletFrequence;

        public XmasBallBehaviour2(Boss boss) : base(boss)
        {
            _screenCenter = new Vector2(
                GameConfig.VirtualResolution.X / 2f,
                GameConfig.VirtualResolution.Y / 2f
            );
        }

        public override void Update(GameTime gameTime)
        {
            if (!Boss.TargetingPosition && !Boss.CurrentAnimator.Position.Equals(_screenCenter))
                Boss.MoveTo(_screenCenter);
            else if (Boss.CurrentAnimator.Position.Equals(_screenCenter))
            {
                if (Boss.CurrentAnimator.CurrentAnimation.Name == "Idle")
                    Boss.CurrentAnimator.Play("Breathe_In");
                else if (Boss.CurrentAnimator.CurrentAnimation.Name == "Big_Idle")
                {
                    Boss.CurrentAnimator.Play("Breathe_Out");
                }
                else if (Boss.CurrentAnimator.CurrentAnimation.Name == "Breathe_Out")
                {
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
    }
}