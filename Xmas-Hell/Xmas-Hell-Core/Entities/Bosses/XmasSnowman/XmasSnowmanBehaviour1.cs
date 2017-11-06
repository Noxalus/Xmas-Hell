using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Timers;
using XmasHell.BulletML;

namespace XmasHell.Entities.Bosses.XmasSnowman
{
    class XmasSnowmanBehaviour1 : AbstractBossBehaviour
    {
        private TimeSpan _newPositionTime;
        private CountdownTimer _shootBulletTimer;

        public XmasSnowmanBehaviour1(Boss boss) : base(boss)
        {
            InitialBehaviourLife = 200;
        }

        public override void Start()
        {
            base.Start();

            Boss.Speed = 500f;
            _newPositionTime = TimeSpan.Zero;

            _shootBulletTimer = new CountdownTimer(1);

            _shootBulletTimer.Completed += (sender, args) =>
            {
                Boss.Game.GameManager.MoverManager.TriggerPattern("XmasSnowman/pattern1", BulletType.Type2, false, Boss.Position());
                _shootBulletTimer.Restart();
            };

            Boss.CurrentAnimator.Play("Idle");
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _shootBulletTimer.Update(gameTime);

            var newPosition = new Vector2(
                Boss.Game.GameManager.Random.Next((int)(Boss.Width() / 2f), GameConfig.VirtualResolution.X - (int)(Boss.Width() / 2f)),
                Boss.Game.GameManager.Random.Next((int)(Boss.Height() / 2f) + 50, (int)(Boss.Height() / 2f) + 150)
            );

            Boss.MoveTo(newPosition, 1.5f);
        }
    }
}