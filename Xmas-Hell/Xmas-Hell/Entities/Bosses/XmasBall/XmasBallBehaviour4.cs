using Microsoft.Xna.Framework;
using Xmas_Hell.Geometry;
using RandomExtension;

namespace Xmas_Hell.Entities.Bosses.XmasBall
{
    class XmasBallBehaviour4 : AbstractBossBehaviour
    {
        public XmasBallBehaviour4(Boss boss) : base(boss)
        {
        }

        public override void Reset()
        {
            base.Reset();

            Boss.Direction = MathHelperExtension.AngleToDirection(
                (float)Boss.Game.GameManager.Random.NextDouble() * MathHelper.Pi
            );

            Boss.Speed = 500f;
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

            if (collision)
            {
                Boss.Acceleration.X = MathHelper.Clamp(Boss.Acceleration.X + 0.1f, 0f, 10f);
                Boss.Acceleration.Y = MathHelper.Clamp(Boss.Acceleration.Y + 0.1f, 0f, 10f);

                Boss.Game.Camera.Shake(0.25f, 20f);
                // TODO: Spawn bullets on each collision
            }
        }
    }
}