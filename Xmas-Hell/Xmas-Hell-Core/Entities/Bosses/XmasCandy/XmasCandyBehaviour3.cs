using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XmasHell.Spriter;
using XmasHell.BulletML;
using SpriterDotNet;
using System.Linq;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandyBehaviour3 : AbstractBossBehaviour
    {
        private List<XmasCandyBar> _candyBars;
        private bool _patternStarted;
        private bool _stretchOut;
        private bool _stretchIn;
        private SpriterFile _body2File;

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

            _candyBars = new List<XmasCandyBar>();

            _patternStarted = false;
            _stretchOut = false;
            _stretchIn = false;

            Boss.StartShootTimer = false;
            Boss.ShootTimerTime = 0.000002f;
            Boss.ShootTimerFinished += ShootTimerFinished;

            _body2File = SpriterUtils.GetSpriterFile("body2.png", Boss.CurrentAnimator);
        }

        private void ShootTimerFinished(object sender, float e)
        {
            if (_candyBars.Count <= 0)
                return;

            var currentCandyBar = _candyBars[0];
            var actionPointPosition = currentCandyBar.ActionPointPosition();

            Boss.Game.GameManager.MoverManager.TriggerPattern("XmasCandy/pattern3", BulletType.Type2, false, actionPointPosition);
        }

        private void EventTriggeredHandler(string eventName)
        {
            if (eventName == "almostFinished")
            {
                // Retrieve the candy bar position
                var worldPosition = SpriterUtils.GetWorldPosition("body2.png", Boss.CurrentAnimator, new Vector2(_body2File.Width / 2f, 0f));
                var angle = Boss.Rotation();

                var candyBarAnimator = Boss.CurrentAnimator.Clone();
                candyBarAnimator.StretchOut = false;
                candyBarAnimator.Position = worldPosition;
                candyBarAnimator.Rotation = angle;

                var candyBar = new XmasCandyBar(Boss, candyBarAnimator);

                _candyBars.Add(candyBar);
            }
        }

        private void GetRandomPositionAndAngle()
        {
            // Retrieve the candy bar position
            var worldPosition = SpriterUtils.GetWorldPosition("body2.png", Boss.CurrentAnimator, new Vector2(_body2File.Width / 2f, 0f));
            var screenSide = Boss.GetSideFromPosition(worldPosition);
            var newRandomPosition = Boss.GetRandomOutsidePosition(screenSide);
            var newRandomAngle = Boss.GetRandomOutsideAngle(screenSide, 50, 90);

            if (screenSide == ScreenSide.Left || screenSide == ScreenSide.Right)
            {
                newRandomPosition.X = (screenSide == ScreenSide.Left) ? 0 : Boss.Game.ViewportAdapter.VirtualWidth;
                Boss.CurrentAnimator.Play("StretchOutBorderWidth");
            }
            else if (screenSide == ScreenSide.Top || screenSide == ScreenSide.Bottom)
            {
                newRandomPosition.Y = (screenSide == ScreenSide.Top) ? 0 : Boss.Game.ViewportAdapter.VirtualHeight;
                Boss.CurrentAnimator.Play("StretchOutBorderHeight");
            }

            Boss.Position(newRandomPosition);
            Boss.Rotation(MathHelper.ToRadians(newRandomAngle));
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

                    _candyBars.First().StartStretchInAnimation();

                    // Hide the boss
                    Boss.Position(new Vector2(-1000, -1000));
                    Boss.CurrentAnimator.Speed = 0;
                    Boss.CurrentAnimator.Progress = 0;
                    Boss.StartShootTimer = true;
                }
            }
        }

        public override void Stop()
        {
            base.Stop();

            foreach (var candy in _candyBars)
                candy.Dispose();

            _candyBars.Clear();

            Boss.CurrentAnimator.AnimationFinished -= AnimationFinishedHandler;
            Boss.CurrentAnimator.EventTriggered -= EventTriggeredHandler;
            Boss.ShootTimerFinished -= ShootTimerFinished;
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
                foreach (var candyBar in _candyBars)
                    candyBar.Update(gameTime);

                var removed = _candyBars.RemoveAll(cb => cb.Destroyed);

                if (removed != 0 && _candyBars.Count > 0)
                {
                    _candyBars.First().StartStretchInAnimation();
                }

                if (_candyBars.Count == 0 && Boss.CurrentAnimator.Speed == 0)
                {
                    _stretchOut = true;
                    _stretchIn = false;
                    GetRandomPositionAndAngle();
                    Boss.CurrentAnimator.Speed = 1;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (var candyBar in _candyBars)
                candyBar.Draw(spriteBatch);
        }
    }
}