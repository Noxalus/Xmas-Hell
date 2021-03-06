using Microsoft.Xna.Framework;
using XmasHell.Extensions;
using XmasHell.Geometry;

namespace XmasHell.Entities.Bosses.XmasBall
{
    class XmasBallBehaviour4 : AbstractBossBehaviour
    {
        private float _initialAcceleration;
        private readonly float _maxAcceleration;

        public XmasBallBehaviour4(Boss boss) : base(boss)
        {
            _maxAcceleration = 3.5f;
        }

        public override void Start()
        {
            base.Start();

            Boss.StopMoving();

            var possibleAngle = new float[4] { 45f, 135f, 225f, 315f };
            Boss.Direction = MathExtension.AngleToDirection(
                MathHelper.ToRadians(possibleAngle[Boss.Game.GameManager.Random.Next(possibleAngle.Length - 1)])
            );

            _initialAcceleration = 1f;
            Boss.Speed = GameConfig.BossDefaultSpeed * 2.5f;
        }

        public override void Stop()
        {
            base.Stop();

            Boss.Acceleration = Vector2.One;
            Boss.CurrentAnimator.Rotation = 0f;
            Boss.Direction = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var currentPosition = Boss.CurrentAnimator.Position;
            var collision = false;
            if (currentPosition.X < Boss.Width() / 2f || currentPosition.X > GameConfig.VirtualResolution.X - Boss.Width() / 2f)
            {
                Boss.Direction.X *= -(float)Boss.Game.GameManager.Random.NextDouble(1f, 1f);
                collision = true;
            }

            if (currentPosition.Y < Boss.Height() / 2f || currentPosition.Y > GameConfig.VirtualResolution.Y - Boss.Height() / 2f)
            {
                Boss.Direction.Y *= -(float)Boss.Game.GameManager.Random.NextDouble(1f, 1f);
                collision = true;
            }

            Boss.CurrentAnimator.Position = new Vector2(
                MathHelper.Clamp(Boss.CurrentAnimator.Position.X, Boss.Width() / 2f,
                GameConfig.VirtualResolution.X - Boss.Width() / 2f),
                MathHelper.Clamp(Boss.CurrentAnimator.Position.Y, Boss.Height() / 2f,
                GameConfig.VirtualResolution.Y - Boss.Height() / 2f)
            );

            Boss.Acceleration = new Vector2(_initialAcceleration + (1 - (CurrentBehaviourLife / InitialBehaviourLife)) * (_maxAcceleration - _initialAcceleration));

            if (collision)
                Boss.Game.Camera.Shake(0.25f, 30f);
        }
    }
}