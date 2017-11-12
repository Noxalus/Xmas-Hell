using System;
using Microsoft.Xna.Framework;
using XmasHell.BulletML;

namespace XmasHell.Entities.Bosses.XmasGift
{
    class XmasGiftBehaviour1 : AbstractBossBehaviour
    {
        public XmasGiftBehaviour1(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            Boss.PhysicsWorld.Gravity = Vector2.Zero;
            Boss.PhysicsEnabled = false;

            base.Start();

            Boss.Speed = GameConfig.BossDefaultSpeed * 2.5f;

            Boss.CurrentAnimator.Play("Idle");

            Boss.StartShootTimer = true;
            Boss.ShootTimerTime = 0.3f;
            Boss.ShootTimerFinished += ShootTimerFinished;

            Boss.Game.GameManager.MoverManager.SetBounceBounds(new Rectangle(0, 0, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y));
        }

        private void ShootTimerFinished(object sender, float e)
        {
            if (Boss.Game.GameManager.GetBossBullets().Count < 20)
                Boss.TriggerPattern("XmasGift/pattern1", BulletType.Type2, false, Boss.ActionPointPosition());
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

            //if (InputManager.KeyDown(Keys.Space))
            {
                //var forceVector = new Vector2(0.5f, 0.5f);
                //var strength = 10f;

                //forceVector.Normalize();
                //_giftBody.ApplyForce(forceVector * strength);
                //_giftBody.ApplyLinearImpulse(new Vector2(100, 10));
                //Boss.PhysicsBody.ApplyAngularImpulse(0.5f);
            }

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