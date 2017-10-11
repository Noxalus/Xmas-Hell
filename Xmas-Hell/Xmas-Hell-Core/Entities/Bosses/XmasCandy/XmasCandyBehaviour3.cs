using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using XmasHell.Spriter;
using System.Diagnostics;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandyBehaviour3 : AbstractBossBehaviour
    {
        private List<CustomSpriterAnimator> _candyBars;
        private bool _patternStarted;

        public XmasCandyBehaviour3(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.Speed = 500f;
            Boss.CurrentAnimator.AnimationFinished += AnimationFinishedHandler;
            _patternStarted = false;
            //Boss.MoveOutside();

            _candyBars = new List<CustomSpriterAnimator>();
        }

        private void AnimationFinishedHandler(string animationName)
        {
            if (animationName == "StretchOutBorderWidth" || animationName == "StretchOutBorderHeight")
            {
                // Retrieve the candy bar position
                var spriteData = Boss.CurrentAnimator.FrameData.SpriteData;

                int folderId = 0;
                var body2File = SpriterUtils.GetSpriterFile("body2.png", Boss.CurrentAnimator, out folderId);
                var worldPosition = SpriterUtils.GetWorldPosition("body2.png", Boss.CurrentAnimator, new Vector2(body2File.Width / 2f, 0f));
                var angle = Boss.Rotation();

                var candyBar = Boss.CurrentAnimator.Clone();
                candyBar.StretchOut = false;
                candyBar.Position = worldPosition;
                candyBar.Rotation = angle;
                candyBar.Progress = 0;
                candyBar.Speed = 1;
                candyBar.Play("StretchInBorderWidth");

                _candyBars.Clear();
                _candyBars.Add(candyBar);

                var bodyFile = SpriterUtils.GetSpriterFile("body.png", Boss.CurrentAnimator, out folderId);
                var bodyPosition = SpriterUtils.GetWorldPosition("body.png", Boss.CurrentAnimator);
                var bodyRotation = Boss.Rotation() + MathHelper.Pi + ((float)Boss.Game.GameManager.Random.NextDouble() * (-(MathHelper.PiOver4 / 1.5f)));

                if (bodyPosition.Y <= 0)
                {
                    bodyPosition.X += Boss.Width() / 2f + body2File.Width;
                    bodyPosition.Y = 0;
                    bodyRotation = -MathHelper.PiOver2;
                }
                else if (bodyPosition.Y > Boss.Game.ViewportAdapter.VirtualHeight)
                {
                    bodyPosition.X -= Boss.Width() / 2f + body2File.Width;
                    bodyPosition.Y = Boss.Game.ViewportAdapter.VirtualHeight;
                    bodyRotation = MathHelper.PiOver2;
                }
                else if (bodyPosition.X > Boss.Game.ViewportAdapter.VirtualWidth)
                {
                    bodyPosition.X = Boss.Game.ViewportAdapter.VirtualWidth;
                    bodyPosition.Y += Boss.Height();
                    bodyRotation = 0;
                }
                else if (bodyPosition.X <= 0)
                {
                    bodyPosition.X = 0;
                    bodyPosition.Y -= Boss.Height();
                    bodyRotation = MathHelper.Pi;
                }

                Boss.Position(bodyPosition);
                Boss.Rotation(bodyRotation);

                //Boss.CurrentAnimator.Play("StretchOutBorderHeight");
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

            if (!_patternStarted /*&& Boss.IsOutside*/)
            {
                // TODO: Choose random initial position
                Boss.Position(new Vector2(Boss.Game.ViewportAdapter.VirtualWidth, Boss.Height()));
                Boss.CurrentAnimator.Play("StretchOutBorderHeight");
                Boss.Rotation((float)Boss.Game.GameManager.Random.NextDouble() * (-(MathHelper.PiOver4) / 2));

                Boss.CurrentAnimator.CurrentAnimation.Looping = false;
                Boss.Position(new Vector2(Boss.Game.ViewportAdapter.VirtualWidth / 3f, 100));
                Boss.Rotation(-MathHelper.PiOver2);

                _patternStarted = true;
            }

            if (_patternStarted)
            {

            }

            foreach (var candyBar in _candyBars)
                candyBar.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (var candyBar in _candyBars)
                candyBar.Draw(spriteBatch);
        }
    }
}