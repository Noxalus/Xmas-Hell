using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using XmasHell.BulletML;

namespace XmasHell.Entities.Bosses.DebugBoss
{
    class BossDebugBehaviour1 : AbstractBossBehaviour
    {
        private TimeSpan _newPositionTime;
        private TimeSpan _bulletFrequence;
        private int _maxBossBullets = 0;
        private bool _triggeredPattern;

        public BossDebugBehaviour1(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            _newPositionTime = TimeSpan.Zero;
            _bulletFrequence = TimeSpan.Zero;
            _triggeredPattern = false;

            Boss.Invincible = true;
            Boss.Speed = 500f;
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

            if (!_triggeredPattern &&
                Boss.Position().X == GameConfig.VirtualResolution.X / 2f &&
                Boss.Position().Y == GameConfig.VirtualResolution.Y / 2f)
            {
                TriggerPattern();
                _triggeredPattern = true;
            }
        }

        private void TriggerPattern()
        {
            Boss.Game.GameManager.MoverManager.TriggerPattern("BossDebug/pattern1", BulletType.Type2, false, Boss.Position());
        }
    }
}