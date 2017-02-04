using System;
using Microsoft.Xna.Framework;
using XmasHell;
using XmasHell.Entities.Bosses;

namespace Xmas_Hell_Core.Entities.Bosses.XmasLog
{
    class XmasLogBehaviour1 : AbstractBossBehaviour
    {
        public XmasLogBehaviour1(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.CurrentAnimator.Play("Whirligig");
            Boss.CurrentAnimator.Speed = 0.25f;
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //if (_currentAnimator.Position.X > GameConfig.VirtualResolution.X)
            //    _direction = -1f;
            //else if (_currentAnimator.Position.X < 0)
            //    _direction = 1f;

            if (Boss.CurrentAnimator.Position.Y > GameConfig.VirtualResolution.Y)
                Boss.Direction = new Vector2(0f, -1f);
            else if (Boss.CurrentAnimator.Position.Y < 0)
                Boss.Direction = new Vector2(0f, 1f);

            Boss.CurrentAnimator.Position += (Boss.Speed * dt) * Boss.Direction;

            Boss.CurrentAnimator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }
    }
}
