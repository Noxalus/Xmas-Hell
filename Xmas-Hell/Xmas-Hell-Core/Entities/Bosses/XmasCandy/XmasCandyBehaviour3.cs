using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XmasHell.Spriter;
using XmasHell.BulletML;
using XmasHell.Extensions;
using SpriterDotNet;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandyBehaviour3 : AbstractBossBehaviour
    {
        private List<CustomSpriterAnimator> _candyBars;
        private List<CustomSpriterAnimator> _candyBarsToRemove;
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

            _candyBars = new List<CustomSpriterAnimator>();
            _candyBarsToRemove = new List<CustomSpriterAnimator>();

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

            foreach (var pointData in currentCandyBar.FrameData.PointData)
            {
                if (pointData.Key.StartsWith("action_point"))
                {
                    var actionPoint = new Vector2(pointData.Value.X, -pointData.Value.Y);
                    var rotatedActionPoint = MathExtension.RotatePoint(actionPoint, currentCandyBar.Rotation);

                    var actionPointPosition = currentCandyBar.Position + rotatedActionPoint;
                    Boss.Game.GameManager.MoverManager.TriggerPattern("XmasCandy/pattern3", BulletType.Type2, false, actionPointPosition);
                    break;
                }
            }

        }

        private void EventTriggeredHandler(string eventName)
        {
            if (eventName == "almostFinished")
            {
                // Retrieve the candy bar position
                var worldPosition = SpriterUtils.GetWorldPosition("body2.png", Boss.CurrentAnimator, new Vector2(_body2File.Width / 2f, 0f));
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

                Boss.Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(candyBar, "body2.png"));

                candyBar.CurrentAnimation.Looping = false;

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
                    _candyBars[0].Speed = 1;
                    Boss.CurrentAnimator.Speed = 0;
                    Boss.CurrentAnimator.Progress = 0;
                    Boss.StartShootTimer = true;
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

            foreach (var candy in _candyBars)
                candy.AnimationFinished -= AnimationFinishedHandler;

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
                    candyBar.Update(gameTime.ElapsedGameTime.Milliseconds);

                foreach (var candyBarToRemove in _candyBarsToRemove)
                    _candyBars.Remove(candyBarToRemove);
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