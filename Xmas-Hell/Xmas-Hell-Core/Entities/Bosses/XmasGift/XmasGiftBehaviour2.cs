using System;
using FarseerPhysics;
using Microsoft.Xna.Framework;

namespace XmasHell.Entities.Bosses.XmasGift
{
    class XmasGiftBehaviour2 : AbstractBossBehaviour
    {
        public XmasGiftBehaviour2(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.PhysicsBody.Position = ConvertUnits.ToSimUnits(Boss.Position());
            Boss.PhysicsWorld.Gravity = GameConfig.DefaultGravity;
            Boss.PhysicsEnabled = true;

            Boss.PhysicsBody.ApplyAngularImpulse(10f);

            Boss.CurrentAnimator.Play("NoAnimation");
        }

        public override void Stop()
        {
            base.Stop();

            Boss.PhysicsWorld.Gravity = Vector2.Zero;
            Boss.PhysicsEnabled = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Boss.Position().Y > Boss.GetPlayerPosition().Y && 
                Math.Abs(Boss.PhysicsBody.LinearVelocity.Y) < 0.0005f && Math.Abs(Boss.PhysicsBody.LinearVelocity.X) < 0.0005f)
            {
                var forceVector = Boss.GetPlayerDirection();
                var strength = 500f;

                forceVector.Normalize();
                //Boss.PhysicsBody.ApplyForce(forceVector * strength);
                Boss.PhysicsBody.ApplyLinearImpulse(forceVector * strength);
                Boss.PhysicsBody.ApplyAngularImpulse(100);//Boss.Game.GameManager.Random.Next(10, 100));
            }
        }
    }
}