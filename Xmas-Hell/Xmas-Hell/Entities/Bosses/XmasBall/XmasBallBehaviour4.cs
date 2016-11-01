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
            Boss.Speed = 1000f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var currentPosition = Boss.CurrentAnimator.Position;
            if (currentPosition.X < Boss.Width() / 2f || currentPosition.X > GameConfig.VirtualResolution.X - Boss.Width() / 2f)
            {
                Boss.Direction.X *= -(float)Boss.Game.GameManager.Random.NextDouble(1f, 1f);
            }

            if (currentPosition.Y < Boss.Height() / 2f || currentPosition.Y > GameConfig.VirtualResolution.Y - Boss.Height() / 2f)
            {
                Boss.Direction.Y *= -(float)Boss.Game.GameManager.Random.NextDouble(1f, 1f);
            }
        }
    }
}