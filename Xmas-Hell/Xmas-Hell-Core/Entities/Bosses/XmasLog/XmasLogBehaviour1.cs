using System;
using Microsoft.Xna.Framework;
using XmasHell;
using XmasHell.Entities.Bosses;

namespace XmasHell.Entities.Bosses.XmasLog
{
    class XmasLogBehaviour1 : AbstractBossBehaviour
    {
        public XmasLogBehaviour1(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.CurrentAnimator.Play("Idle");
            Boss.MoveToCenter();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
