using System;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace XmasHell.Entities.Bosses.XmasGift
{
    class XmasGiftBehaviour2 : AbstractBossBehaviour
    {
        private TimeSpan _impulseTimer;

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

            Boss.PhysicsBody.OnCollision += OnCollision;

            Boss.CurrentAnimator.Play("NoAnimation");
            _impulseTimer = TimeSpan.Zero;
        }

        private bool OnCollision(FarseerPhysics.Dynamics.Fixture fixtureA, FarseerPhysics.Dynamics.Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            Debug.WriteLine("Linear velocity: " + Boss.PhysicsBody.LinearVelocity);

            if (Boss.Position().Y > Boss.GetPlayerPosition().Y)
                ImpulseToPlayer();

            return true;
        }

        public override void Stop()
        {
            base.Stop();

            Boss.PhysicsWorld.Gravity = Vector2.Zero;
            Boss.PhysicsEnabled = false;
            Boss.PhysicsBody.OnCollision -= OnCollision;
        }

        private void ImpulseToPlayer()
        {
            if (_impulseTimer.TotalMilliseconds > 0)
                return;

            var forceVector = Boss.GetPlayerDirection();
            var strength = 500f;

            forceVector.Normalize();
            //Boss.PhysicsBody.ApplyForce(forceVector * strength);
            Boss.PhysicsBody.ApplyLinearImpulse(forceVector * strength);
            Boss.PhysicsBody.ApplyAngularImpulse(100);//Boss.Game.GameManager.Random.Next(10, 100));

            _impulseTimer = TimeSpan.FromSeconds(0.1);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Boss.Position().Y > Boss.GetPlayerPosition().Y &&
                Math.Abs(Boss.PhysicsBody.LinearVelocity.Y) < 0.0005f && Math.Abs(Boss.PhysicsBody.LinearVelocity.X) < 0.0005f)
            {
                ImpulseToPlayer();
            }

            if (_impulseTimer.TotalMilliseconds > 0)
                _impulseTimer -= gameTime.ElapsedGameTime;
        }
    }
}