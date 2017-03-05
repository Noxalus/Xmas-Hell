using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Timers;
using XmasHell.BulletML;

namespace XmasHell.Entities.Bosses.XmasSnowflake
{
    class XmasSnowflakeBehaviour1 : AbstractBossBehaviour
    {
        private TimeSpan _newPositionTime;
        private CountdownTimer _shootBulletTimer;

        public XmasSnowflakeBehaviour1(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.Speed = 500f;
            _newPositionTime = TimeSpan.Zero;

            _shootBulletTimer = new CountdownTimer(1);

            _shootBulletTimer.Completed += (sender, args) =>
            {
                //Boss.Game.GameManager.MoverManager.TriggerPattern("XmasSnowflake/pattern1", BulletType.Type2, false, Boss.Position());

                //_shootBulletTimer.Interval = TimeSpan.FromSeconds(5);
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
                    Boss.Game.GameManager.Random.Next((int)(Boss.Height() / 2f) + 42, 288 - (int)(Boss.Height() / 2f))
                );

                Boss.MoveTo(newPosition, 1.5f);
            }
        }
    }
}