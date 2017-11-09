using Microsoft.Xna.Framework;
using XmasHell.BulletML;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandyBehaviour1 : AbstractBossBehaviour
    {
        public XmasCandyBehaviour1(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.Speed = GameConfig.BossDefaultSpeed * 2.5f;

            Boss.CurrentAnimator.Play("Idle");

            Boss.StartShootTimer = true;
            Boss.ShootTimerTime = 3f;
            Boss.ShootTimerFinished += ShootTimerFinished;
        }

        private void ShootTimerFinished(object sender, float e)
        {
            Boss.TriggerPattern("XmasCandy/pattern1", BulletType.Type2, false, Boss.ActionPointPosition());
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

            if (!Boss.TargetingPosition)
            {
                var newPosition = new Vector2(
                    Boss.Game.GameManager.Random.Next((int)(Boss.Width() / 2f), GameConfig.VirtualResolution.X - (int)(Boss.Width() / 2f)),
                    Boss.Game.GameManager.Random.Next((int)(Boss.Height() / 2f) + 150, 500 - (int)(Boss.Height() / 2f))
                );

                Boss.MoveTo(newPosition, 1.5f);
            }
        }
    }
}