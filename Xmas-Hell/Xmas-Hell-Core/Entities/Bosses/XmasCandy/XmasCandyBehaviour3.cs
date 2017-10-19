using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XmasHell.Spriter;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandyBehaviour3 : AbstractBossBehaviour
    {
        private List<CustomSpriterAnimator> _candyBars;
        private List<CustomSpriterAnimator> _candyBarsToRemove;
        private bool _patternStarted;
        private bool _stretchOut;
        private bool _stretchIn;

        public XmasCandyBehaviour3(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.Speed = 500f;
            Boss.CurrentAnimator.AnimationFinished += AnimationFinishedHandler;
            Boss.CurrentAnimator.EventTriggered += EventTriggeredHandler;
            Boss.MoveOutside(true);

            _candyBars = new List<CustomSpriterAnimator>();
            _candyBarsToRemove = new List<CustomSpriterAnimator>();

            _patternStarted = false;
            _stretchOut = false;
            _stretchIn = false;
        }

        private void EventTriggeredHandler(string eventName)
        {
            if (eventName == "almostFinished")
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
                candyBar.Speed = 0;
                candyBar.AnimationFinished += AnimationFinishedHandler;

                var currentSide = Boss.GetSideFromPosition(Boss.Position());

                if (currentSide == ScreenSide.Left || currentSide == ScreenSide.Right)
                    candyBar.Play("StretchInBorderWidth");
                else
                    candyBar.Play("StretchInBorderHeight");

                candyBar.CurrentAnimation.Looping = false;

                _candyBars.Add(candyBar);
            }
        }

        private void GetRandomPositionAndAngle()
        {
            // Retrieve the candy bar position
            var spriteData = Boss.CurrentAnimator.FrameData.SpriteData;
            int folderId = 0;
            var body2File = SpriterUtils.GetSpriterFile("body2.png", Boss.CurrentAnimator, out folderId);
            var worldPosition = SpriterUtils.GetWorldPosition("body2.png", Boss.CurrentAnimator, new Vector2(body2File.Width / 2f, 0f));
            var screenSide = Boss.GetSideFromPosition(worldPosition);
            var newRandomPosition = Boss.GetRandomOutsidePosition(screenSide);

            switch (screenSide)
            {
                case ScreenSide.Left:
                    newRandomPosition.X = -Boss.Width();
                    newRandomPosition.Y -= Boss.Height();
                    Boss.CurrentAnimator.Play("StretchOutBorderWidth");
                    break;
                case ScreenSide.Top:
                    newRandomPosition.X += Boss.Width() / 2f + body2File.Width;
                    newRandomPosition.Y = -Boss.Height();
                    Boss.CurrentAnimator.Play("StretchOutBorderHeight");
                    break;
                case ScreenSide.Right:
                    newRandomPosition.X = Boss.Game.ViewportAdapter.VirtualWidth + Boss.Width();
                    newRandomPosition.Y += Boss.Height();
                    Boss.CurrentAnimator.Play("StretchOutBorderWidth");
                    break;
                case ScreenSide.Bottom:
                    newRandomPosition.X -= Boss.Width() / 2f + body2File.Width;
                    newRandomPosition.Y = Boss.Game.ViewportAdapter.VirtualHeight + Boss.Height();
                    Boss.CurrentAnimator.Play("StretchOutBorderHeight");
                    break;
                default:
                    break;
            }

            Boss.Position(newRandomPosition);
            Boss.Rotation(MathHelper.ToRadians(Boss.GetRandomOutsideAngle(screenSide, 50, 90)));
        }

        private void AnimationFinishedHandler(string animationName)
        {
            if (animationName == "StretchOutBorderWidth" || animationName == "StretchOutBorderHeight")
            {
                if (_candyBars.Count < 5)
                    GetRandomPositionAndAngle();
                else
                {
                    _stretchIn = true;
                    _candyBars[0].Speed = 1;
                    Boss.CurrentAnimator.Speed = 0;
                    Boss.CurrentAnimator.Progress = 0;
                }
            }
            else if (animationName == "StretchInBorderWidth" || animationName == "StretchInBorderHeight")
            {
                if (_candyBars.Count > 0)
                {
                    var candyBar = _candyBars[0];
                    candyBar.AnimationFinished -= AnimationFinishedHandler;
                    _candyBarsToRemove.Add(candyBar);
                }

                if (_candyBars.Count > 1)
                    _candyBars[1].Speed = 1;
                else
                {
                    _stretchOut = true;
                    _stretchIn = false;
                    GetRandomPositionAndAngle();
                    Boss.CurrentAnimator.Speed = 1;
                }
            }
        }

        public override void Stop()
        {
            base.Stop();

            Boss.CurrentAnimator.AnimationFinished -= AnimationFinishedHandler;
            Boss.CurrentAnimator.EventTriggered -= EventTriggeredHandler;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!_patternStarted && !Boss.TargetingPosition && Boss.IsOutside)
            {
                GetRandomPositionAndAngle();
                _patternStarted = true;
            }

            if (_patternStarted)
            {
            }

            foreach (var candyBar in _candyBars)
                candyBar.Update(gameTime.ElapsedGameTime.Milliseconds);

            foreach (var candyBarToRemove in _candyBarsToRemove)
                _candyBars.Remove(candyBarToRemove);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (var candyBar in _candyBars)
                candyBar.Draw(spriteBatch);
        }
    }
}