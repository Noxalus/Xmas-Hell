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
            InitialBehaviourLife = GameConfig.BossDefaultBehaviourLife * 0.5f;
        }

        public override void Start()
        {
            base.Start();

            Boss.Speed = GameConfig.BossDefaultSpeed * 2.5f;

            Boss.StartShootTimer = true;
            Boss.ShootTimerTime = 0.001f;
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
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //var newPosition = new Vector2(
            //    Boss.Game.GameManager.Random.Next((int)(Boss.Width() / 2f), GameConfig.VirtualResolution.X - (int)(Boss.Width() / 2f)),
            //    Boss.Game.GameManager.Random.Next((int)(Boss.Height() / 2f) + 50, (int)(Boss.Height() / 2f) + 150)
            //);

            //Boss.MoveTo(newPosition, 1.5f);
        }
    }
}