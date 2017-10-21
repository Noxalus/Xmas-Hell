using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using XmasHell.Spriter;
using XmasHell.BulletML;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandyBehaviour4 : AbstractBossBehaviour
    {
        public XmasCandyBehaviour4(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            Boss.CurrentAnimator.StretchOut = true;
            Boss.Game.GameManager.CollisionWorld.ClearBossHitboxes();
            Boss.Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(170f, 0f), 0.2f));
            Boss.Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(-170f, 0f), 0.2f));

            Boss.CurrentAnimator.Scale = new Vector2(5f, 5f);

            base.Start();

            Boss.Speed = 200f;
            Boss.CurrentAnimator.AnimationFinished += AnimationFinishedHandler;
            Boss.CurrentAnimator.Play("Circle");
            Boss.CurrentAnimator.Speed = 1f;

            Boss.Position(Boss.GetPlayerPosition());

            Boss.StartShootTimer = true;
            Boss.ShootTimerTime = 3f;
            Boss.ShootTimerFinished += ShootTimerFinished;
        }

        private void AnimationFinishedHandler(string animationName)
        {
        }

        private void ShootTimerFinished(object sender, float e)
        {
            Boss.TriggerPattern("XmasCandy/pattern4", BulletType.Type2, false, Boss.ActionPointPosition());
        }

        public override void Stop()
        {
            base.Stop();

            Boss.CurrentAnimator.AnimationFinished -= AnimationFinishedHandler;
            Boss.StartShootTimer = false;
            Boss.ShootTimerFinished -= ShootTimerFinished;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Boss.Scale(Boss.Scale() - new Vector2(0.01f));

            Boss.MoveTo(Boss.GetPlayerPosition(), true);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}