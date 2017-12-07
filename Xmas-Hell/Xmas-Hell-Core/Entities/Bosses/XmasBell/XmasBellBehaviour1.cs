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

            Boss.Speed = GameConfig.BossDefaultSpeed * 2.5f;
            Boss.CurrentAnimator.Play("Idle");
            Boss.EnableRandomPosition(true, true);
            Boss.StartShootTimer = true;
            Boss.ShootTimerFinished += ShootTimerFinished;
            Boss.ShootTimerTime = 0.5f;
        }

        private void ShootTimerFinished(object sender, float e)
        {
            Boss.Game.GameManager.MoverManager.TriggerPattern("XmasBell/pattern1", BulletType.Type2, false, Boss.ActionPointPosition());
        }

        public override void Stop()
        {
            base.Stop();

            Boss.EnableRandomPosition(false);
            Boss.StartShootTimer = false;
            Boss.ShootTimerFinished -= ShootTimerFinished;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}