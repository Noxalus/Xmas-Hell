using System;
using Microsoft.Xna.Framework;
using RandomExtension;
using XmasHell.BulletML;

namespace XmasHell.Entities.Bosses.XmasBell
{
    class XmasBellBehaviour3 : AbstractBossBehaviour
    {
        private TimeSpan _timeBeforeShoot;

        public XmasBellBehaviour3(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.Speed = 500f;
            _timeBeforeShoot = TimeSpan.FromSeconds(2);

            Boss.MoveTo(
                new Vector2(
                    GameConfig.VirtualResolution.X / 2f - Boss.Width() / 2f,
                    Boss.Height()
                ),
                true
            );

            Boss.CurrentAnimator.Play("Idle");
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Boss.TargetingPosition)
            {
                // Follow the player
                Boss.MoveTo(
                    new Vector2(
                        Boss.GetPlayerPosition().X,
                        Boss.CurrentAnimator.Position.Y
                    ),
                    true
                );

                if (_timeBeforeShoot.TotalMilliseconds < 0)
                {
                    _timeBeforeShoot = TimeSpan.FromSeconds(
                        Boss.Game.GameManager.Random.NextFloat(0.5f, 5f)
                    );

                    Boss.CurrentAnimator.Transition("Shoot", 0.5f);

                    Boss.TriggerPattern("XmasBell/pattern2", BulletType.Type3);
                }
                else
                {
                    _timeBeforeShoot -= gameTime.ElapsedGameTime;
                }

            }
        }
    }
}