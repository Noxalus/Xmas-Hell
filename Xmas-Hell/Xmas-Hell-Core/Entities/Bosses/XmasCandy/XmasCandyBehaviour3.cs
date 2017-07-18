using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandyBehaviour3 : AbstractBossBehaviour
    {
        private List<XmasCandyBar> _candyBars;

        public XmasCandyBehaviour3(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.CurrentAnimator.Play("StretchOutBorder");

            Boss.CurrentAnimator.AnimationFinished += AnimationFinishedHandler;

            _candyBars = new List<XmasCandyBar>();
        }

        private void AnimationFinishedHandler(string animationName)
        {
            if (animationName == "StretchOutBorder")
            {
                var candyBarSprite = new Sprite(Assets.GetTexture2D("Graphics/Sprites/Bosses/XmasCandy/candy-bar"));
                candyBarSprite.Origin = new Vector2(candyBarSprite.BoundingRectangle.Width / 2f, 0f);
                candyBarSprite.Rotation = -MathHelper.PiOver2;

                // Retrieve the candy bar position
                var spriteData = Boss.CurrentAnimator.FrameData.SpriteData;

                var body2File = Spriter.SpriterUtils.GetSpriterFile("body2.png", Boss.CurrentAnimator);
                var body2Sprite = spriteData.FindAll(so => so.FileId == body2File.Id)[0];

                var worldPosition = Spriter.SpriterUtils.GetSpriterWorldPosition(body2Sprite, Boss.CurrentAnimator);
                var angle = MathHelper.ToRadians(body2Sprite.Angle);

                candyBarSprite.Position = new Vector2(body2Sprite.X, body2Sprite.Y);
                candyBarSprite.Rotation = angle;

                var candyBar = new XmasCandyBar(Boss, candyBarSprite);

                _candyBars.Add(candyBar);
            }
        }

        public override void Stop()
        {
            base.Stop();

            Boss.CurrentAnimator.AnimationFinished -= AnimationFinishedHandler;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var candyBar in _candyBars)
                candyBar.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (var candyBar in _candyBars)
                candyBar.Draw(spriteBatch);
        }
    }
}