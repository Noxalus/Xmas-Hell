using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Timers;
using XmasHell.BulletML;

namespace XmasHell.Entities.Bosses.XmasTree
{
    class XmasTreeBehaviour1 : AbstractBossBehaviour
    {
        public XmasTreeBehaviour1(Boss boss) : base(boss)
        {
            InitialBehaviourLife = GameConfig.BossDefaultBehaviourLife * 1.5f;
        }

        public override void Start()
        {
            base.Start();

            Boss.Speed = GameConfig.BossDefaultSpeed * 2.5f;

            Boss.StartShootTimer = true;
            Boss.ShootTimerTime = 0.01f;
            Boss.ShootTimerFinished += ShootTimerFinished;

            Boss.CurrentAnimator.Play("Idle");
        }

        private void ShootTimerFinished(object sender, float e)
        {
            Boss.Game.GameManager.MoverManager.TriggerPattern("XmasTree/pattern1", BulletType.Type2, false, Boss.Position());
        }

        public override void Stop()
        {
            base.Stop();

            Boss.StartShootTimer = false;
            Boss.ShootTimerFinished -= ShootTimerFinished;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}