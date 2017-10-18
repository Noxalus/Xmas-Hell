using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XmasHell.Spriter;

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

                var screenSide = Boss.GetSideFromPosition(bodyPosition);

                var newRandomPosition = Boss.GetRandomOutsidePosition(screenSide);

                switch (screenSide)
                {
                    case ScreenSide.Left:
                        newRandomPosition.X = 0;
                        newRandomPosition.Y -= Boss.Height();
                        Boss.CurrentAnimator.Play("StretchOutBorderWidth");
                        break;
                    case ScreenSide.Top:
                        newRandomPosition.X += Boss.Width() / 2f + body2File.Width;
                        newRandomPosition.Y = 0;
                        Boss.CurrentAnimator.Play("StretchOutBorderHeight");
                        break;
                    case ScreenSide.Right:
                        newRandomPosition.X = Boss.Game.ViewportAdapter.VirtualWidth;
                        newRandomPosition.Y += Boss.Height();
                        Boss.CurrentAnimator.Play("StretchOutBorderWidth");
                        break;
                    case ScreenSide.Bottom:
                        newRandomPosition.X -= Boss.Width() / 2f + body2File.Width;
                        newRandomPosition.Y = Boss.Game.ViewportAdapter.VirtualHeight;
                        Boss.CurrentAnimator.Play("StretchOutBorderHeight");
                        break;
                    default:
                        break;
                }

                Boss.Position(newRandomPosition);
                Boss.Rotation(MathHelper.ToRadians(Boss.GetRandomOutsideAngle(screenSide, 50, 90)));
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
                //Boss.Position(new Vector2(Boss.Game.ViewportAdapter.VirtualWidth, Boss.Height()));
                //Boss.CurrentAnimator.Play("StretchOutBorderHeight");
                //Boss.Rotation((float)Boss.Game.GameManager.Random.NextDouble() * (-(MathHelper.PiOver4) / 2));

                Boss.CurrentAnimator.CurrentAnimation.Looping = false;
                //Boss.Position(new Vector2(Boss.Game.ViewportAdapter.VirtualWidth / 3f, 100));
                //Boss.Rotation(-MathHelper.PiOver2);

                var nearestPosition = Boss.GetNearestOutsidePosition();
                var side = Boss.GetSideFromPosition(nearestPosition);
                Boss.Position(Boss.GetNearestOutsidePosition());
                Boss.Position(new Vector2(Boss.Position().X, Boss.Game.ViewportAdapter.VirtualHeight));
                Boss.Rotation(MathHelper.ToRadians(Boss.GetRandomOutsideAngle(Boss.GetSideFromPosition(Boss.Position()), 10, 90)));
                Boss.CurrentAnimator.Play("StretchOutBorderHeight");

                _patternStarted = true;
            }

            if (_patternStarted)
            {
                //Boss.Rotation(MathHelper.ToRadians(Boss.GetRandomOutsideAngle(Boss.GetSideFromPosition(Boss.Position()), 10)));
                //Boss.Rotation(MathHelper.ToRadians(Boss.GetRandomOutsideAngle(ScreenSide.Bottom, 10, 90)));
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