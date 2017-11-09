using Microsoft.Xna.Framework;
using MonoGame.Extended.Timers;
using XmasHell.BulletML;

namespace XmasHell.Entities.Bosses.XmasSnowflake
{
    class XmasSnowflakeBehaviour3 : AbstractBossBehaviour
    {
        private CountdownTimer _shootBulletTimer;

        public XmasSnowflakeBehaviour3(Boss boss) : base(boss)
        {
            InitialBehaviourLife = GameConfig.BossDefaultBehaviourLife * 0.75f;

        }

        public override void Start()
        {
            base.Start();

            Boss.CurrentAnimator.Play("Idle");

            _shootBulletTimer = new CountdownTimer(0.02);

            _shootBulletTimer.Completed += (sender, args) =>
            {
                ShootRainBullet();

                _shootBulletTimer.Restart();
            };
        }

        public override void Stop()
        {
            base.Stop();
        }

        private void ShootRainBullet()
        {
            Vector2 randomPosition;

            if (Boss.Game.GameManager.Random.NextDouble() > 0.5)
                randomPosition = GetRandomLeftWallPosition();
            else
                randomPosition = GetRandomTopWallPosition();

            Boss.Game.GameManager.MoverManager.TriggerPattern(
                "XmasSnowflake/pattern3",
                BulletType.Type2,
                false,
                randomPosition
            );
        }

        private Vector2 GetRandomLeftWallPosition()
        {
            return new Vector2(
                -100,
                (float)Boss.Game.GameManager.Random.NextDouble() * GameConfig.VirtualResolution.Y
            );
        }

        private Vector2 GetRandomTopWallPosition()
        {
            return new Vector2(
                (float)Boss.Game.GameManager.Random.NextDouble() * GameConfig.VirtualResolution.X,
                -100
            );
        }

        public override void Update(GameTime gameTime)
        {
            var newPosition = new Vector2(
                Boss.Game.GameManager.Random.Next((int)(Boss.Width() / 2f), GameConfig.VirtualResolution.X - (int)(Boss.Width() / 2f)),
                Boss.Game.GameManager.Random.Next((int)(Boss.Height() / 2f) + 42, 288 - (int)(Boss.Height() / 2f))
            );

            Boss.MoveTo(newPosition, 1.5f);

            _shootBulletTimer.Update(gameTime);

            base.Update(gameTime);
        }
    }
}