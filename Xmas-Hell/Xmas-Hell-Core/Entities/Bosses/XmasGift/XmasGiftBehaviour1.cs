using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using XmasHell.BulletML;

namespace XmasHell.Entities.Bosses.XmasGift
{
    class XmasGiftBehaviour1 : AbstractBossBehaviour
    {
        private TimeSpan _newPositionTime;
        private TimeSpan _bulletFrequence;

        public XmasGiftBehaviour1(Boss boss) : base(boss)
        {
            InitialBehaviourLife = 100f;
        }

        public override void Start()
        {
            base.Start();

            Boss.Speed = 500f;
            _newPositionTime = TimeSpan.Zero;
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

            //if (InputManager.KeyDown(Keys.Space))
            {
                //var forceVector = new Vector2(0.5f, 0.5f);
                //var strength = 10f;

                //forceVector.Normalize();
                //_giftBody.ApplyForce(forceVector * strength);
                //_giftBody.ApplyLinearImpulse(new Vector2(100, 10));
                Boss.PhysicsBody.ApplyAngularImpulse(0.5f);
            }

            //if (_newPositionTime.TotalMilliseconds > 0)
            //{
            //    if (!Boss.TargetingPosition)
            //        _newPositionTime -= gameTime.ElapsedGameTime;
            //}
            //else
            //{
            //    _newPositionTime = TimeSpan.FromSeconds(0);

            //    var newPosition = new Vector2(
            //        Boss.Game.GameManager.Random.Next((int)(Boss.Width() / 2f), GameConfig.VirtualResolution.X - (int)(Boss.Width() / 2f)),
            //        Boss.Game.GameManager.Random.Next((int)(Boss.Height() / 2f) + 42, 288 - (int)(Boss.Height() / 2f))
            //    );

            //    Boss.MoveTo(newPosition, 1.5f);
            //}

            //if (_bulletFrequence.TotalMilliseconds > 0)
            //    _bulletFrequence -= gameTime.ElapsedGameTime;
            //else
            //{
            //    _bulletFrequence = TimeSpan.FromSeconds(0.5f);
            //    Boss.Game.GameManager.MoverManager.TriggerPattern("sample", BulletType.Type2, false, Boss.Position());
            //}
        }
    }
}