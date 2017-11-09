using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Timers;
using XmasHell.BulletML;

namespace XmasHell.Entities.Bosses.XmasReindeer
{
    class XmasReindeerBehaviour1 : AbstractBossBehaviour
    {
        private CountdownTimer _shootBulletTimer;

        public XmasReindeerBehaviour1(Boss boss) : base(boss)
        {
            InitialBehaviourLife = GameConfig.BossDefaultBehaviourLife * 0.5f;
        }

        public override void Start()
        {
            base.Start();

            Boss.Speed = 500f;

            _shootBulletTimer = new CountdownTimer(1);
            _shootBulletTimer.Completed += (sender, args) =>
            {
                Boss.Game.GameManager.MoverManager.TriggerPattern("XmasReindeer/pattern1", BulletType.Type2, false, Boss.Position());
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
                Boss.Game.GameManager.Random.Next((int)(Boss.Height() / 2f) + 200, (int)(Boss.Height() / 2f) + 300)
            );

            Boss.MoveTo(newPosition, 1.5f);
        }
    }
}