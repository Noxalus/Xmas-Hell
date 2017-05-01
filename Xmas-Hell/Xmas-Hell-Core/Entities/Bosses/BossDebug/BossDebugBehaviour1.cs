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

        public BossDebugBehaviour1(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            _newPositionTime = TimeSpan.Zero;
            _bulletFrequence = TimeSpan.Zero;

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

            //FindNewPosition(gameTime);

            if (Boss.Game._fpsCounter.FramesPerSecond > 30)
            {
                _maxBossBullets = Boss.Game.GameManager.GetBossBullets().Count;
                ShootBullet(gameTime);
            }
            else
            {
                Debug.WriteLine("Max boss bullets: " + _maxBossBullets);
            }
        }

        private void FindNewPosition(GameTime gameTime)
        {
            if (_newPositionTime.TotalMilliseconds > 0)
            {
                if (!Boss.TargetingPosition)
                    _newPositionTime -= gameTime.ElapsedGameTime;
            }
            else
            {
                _newPositionTime = TimeSpan.FromSeconds(0);

                var newPosition = new Vector2(
                    Boss.Game.GameManager.Random.Next((int)(Boss.Width() / 2f), GameConfig.VirtualResolution.X - (int)(Boss.Width() / 2f)),
                    Boss.Game.GameManager.Random.Next((int)(Boss.Height() / 2f) + 100, 500 - (int)(Boss.Height() / 2f))
                );

                Boss.MoveTo(newPosition, 1.5f);
            }
        }

        private void ShootBullet(GameTime gameTime)
        {
            Boss.Game.GameManager.MoverManager.TriggerPattern("BossDebug/pattern1", BulletType.Type2, false, Boss.Position());
            //if (_bulletFrequence.TotalMilliseconds > 0)
            //    _bulletFrequence -= gameTime.ElapsedGameTime;
            //else
            //{
            //    _bulletFrequence = TimeSpan.FromSeconds(0.00000005f);
            //    Boss.Game.GameManager.MoverManager.TriggerPattern("DebugBoss/pattern1", BulletType.Type2, false, Boss.Position());
            //}
        }
    }
}