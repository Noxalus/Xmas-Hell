using System;
using Microsoft.Xna.Framework;
using XmasHell.BulletML;

namespace XmasHell.Entities.Bosses.XmasBell
{
    class XmasBellBehaviour1 : AbstractBossBehaviour
    {
        private TimeSpan _bulletFrequence;

        public XmasBellBehaviour1(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.Speed = 500f;
            _bulletFrequence = TimeSpan.Zero;

            Boss.CurrentAnimator.Play("Idle");
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var newPosition = new Vector2(
                Boss.Game.GameManager.Random.Next(
                    (int)(Boss.Width() / 2f),
                    (int)(Boss.Game.ViewportAdapter.VirtualWidth - (Boss.Width() / 2f))
                ),
                (int)(Boss.Height() / 1.25f)
            );

            Boss.MoveTo(newPosition, 1.5f);

            if (_bulletFrequence.TotalMilliseconds > 0)
                _bulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                _bulletFrequence = TimeSpan.FromSeconds(0.5f);
                Boss.Game.GameManager.MoverManager.TriggerPattern("XmasBell/pattern1", BulletType.Type2, false, Boss.ActionPointPosition());
            }
        }
    }
}