using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Diagnostics;
using XmasHell.BulletML;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandyBehaviour4 : AbstractBossBehaviour
    {
        private float _minScale = 0.1f;
        private float _maxRotationSpeed = 3f;
        private float _minRotationSpeed = 1f;
        private float _maxShootTimeFrequence = 0.01f;
        private float _minShootTimeFrequence = 0.25f;

        public XmasCandyBehaviour4(Boss boss) : base(boss)
        {
            InitialBehaviourLife = GameConfig.BossDefaultBehaviourLife * 4.5f;
        }

        public override void Start()
        {
            Boss.CurrentAnimator.StretchOut = true;
            Boss.ClearHitBoxes();

            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(1800f, 0f), 0.2f));
            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(1600f, 800f), 0.2f));
            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(1100f, 1400f), 0.2f));
            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(400f, 1800f), 0.2f));

            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(1800f, 0f), 0.2f));
            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(1600f, -800f), 0.2f));
            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(1100f, -1400f), 0.2f));
            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(400f, -1800f), 0.2f));

            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(-1800f, 0f), 0.2f));
            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(-1600f, 800f), 0.2f));
            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(-1100f, 1400f), 0.2f));
            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(-400f, 1800f), 0.2f));

            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(-1800f, 0f), 0.2f));
            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(-1600f, -800f), 0.2f));
            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(-1100f, -1400f), 0.2f));
            Boss.AddHitBox(new SpriterCollisionCircle(Boss, "body4.png", new Vector2(-400f, -1800f), 0.2f));

            Boss.CurrentAnimator.Scale = new Vector2(1f);

            base.Start();

            Boss.Speed = GameConfig.BossDefaultSpeed * 0.5f;
            Boss.CurrentAnimator.AnimationFinished += AnimationFinishedHandler;
            Boss.CurrentAnimator.Play("CircleAppears");
            Boss.CurrentAnimator.Speed = _maxRotationSpeed;

            Boss.Position(Boss.GetPlayerPosition());

            Boss.StartShootTimer = false;
            Boss.ShootTimerTime = _maxShootTimeFrequence;
            Boss.ShootTimerFinished += ShootTimerFinished;
        }

        private void AnimationFinishedHandler(string animationName)
        {
            if (animationName == "CircleAppears")
            {
                Boss.CurrentAnimator.Play("Circle");
                Boss.StartShootTimer = true;
            }
        }

        private void ShootTimerFinished(object sender, float e)
        {
            // Bullet speed should depends on the boss scale
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

            if (Boss.StartShootTimer && Boss.ScaleVector().X > _minScale && Boss.ScaleVector().Y > _minScale)
            {
                var factor = 0.01f * gameTime.GetElapsedSeconds();
                Boss.CurrentAnimator.Speed -= factor * 1.5f;
                Boss.ShootTimerTime = MathHelper.Clamp(Boss.ShootTimerTime + factor, _maxShootTimeFrequence, _minShootTimeFrequence);
                Boss.Scale(Boss.ScaleVector() - new Vector2(factor));
            }

            Boss.MoveTo(Boss.GetPlayerPosition(), true);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}