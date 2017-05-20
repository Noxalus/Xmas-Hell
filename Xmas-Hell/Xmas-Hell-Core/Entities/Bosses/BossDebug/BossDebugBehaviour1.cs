using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using XmasHell.BulletML;
using XmasHell.Geometry;

namespace XmasHell.Entities.Bosses.DebugBoss
{
    class BossDebugBehaviour1 : AbstractBossBehaviour
    {
        private TimeSpan _newPositionTime;
        private TimeSpan _bulletFrequence;
        private bool _triggeredPattern;
        private Laser _laser;

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

            _laser = new Laser(Boss.Game, new Line(Boss.Game.ViewportAdapter.Center.ToVector2(), Boss.Game.ViewportAdapter.Center.ToVector2() + new Vector2(100f, 0f)));

            Boss.Game.GameManager.AddLaser(_laser);
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

            _laser.Update(gameTime);
        }

        private void TriggerPattern()
        {
            Boss.Game.GameManager.MoverManager.TriggerPattern("BossDebug/pattern1", BulletType.Type2, false, Boss.Position());
        }
    }
}