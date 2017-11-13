using System;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XmasHell.BulletML;
using XmasHell.Controls;

namespace XmasHell.Entities.Bosses.XmasGift
{
    class XmasGiftBehaviour2 : AbstractBossBehaviour
    {
        public XmasGiftBehaviour2(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            Boss.PhysicsBody.Position = ConvertUnits.ToSimUnits(Boss.Position());
            Boss.PhysicsWorld.Gravity = GameConfig.DefaultGravity;
            Boss.PhysicsEnabled = true;

            base.Start();

            Boss.CurrentAnimator.Play("NoAnimation");
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.KeyPressed(Keys.Space))
            {
                var forceVector = new Vector2(0.5f, -0.5f);
                var strength = 1000f;

                forceVector.Normalize();
                //Boss.PhysicsBody.ApplyForce(forceVector * strength);
                Boss.PhysicsBody.ApplyLinearImpulse(forceVector * strength);
                Boss.PhysicsBody.ApplyAngularImpulse(100f);
            }
        }
    }
}