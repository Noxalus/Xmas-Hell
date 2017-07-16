using Microsoft.Xna.Framework;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandyBehaviour1 : AbstractBossBehaviour
    {
        public XmasCandyBehaviour1(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.Invincible = true;
            Boss.Speed = 500f;

            Boss.CurrentAnimator.Play("StretchOut");
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Boss.Rotation(Boss.Rotation() + 0.01f);
        }
    }
}