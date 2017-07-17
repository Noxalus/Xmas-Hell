using System;
using Microsoft.Xna.Framework;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandyBehaviour2 : AbstractBossBehaviour
    {
        private bool _targetingPlayer;
        private bool _stretchingOut;
        private TimeSpan _targetingPlayerTimer;
        private TimeSpan _stretchingOutTimer;

        public XmasCandyBehaviour2(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.Speed = 500f;

            ResetStretchOutAttack();
            Boss.CurrentAnimator.AnimationFinished += AnimationFinishedHandler;
        }

        private void ResetStretchOutAttack()
        {
            Boss.RotateTo(0);
            Boss.CurrentAnimator.Play("Idle");
            _targetingPlayer = false;
            _stretchingOut = false;
            _targetingPlayerTimer = TimeSpan.FromSeconds(Boss.Game.GameManager.Random.NextDouble() * 0.5f);
            _stretchingOutTimer = TimeSpan.FromSeconds(Boss.Game.GameManager.Random.Next(2, 8));
        }

        private void AnimationFinishedHandler(string animationName)
        {
            if (animationName == "StretchOut")
                ResetStretchOutAttack();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!_targetingPlayer && !_stretchingOut && !Boss.TargetingPosition)
            {
                var newPosition = new Vector2(
                    Boss.Game.GameManager.Random.Next((int)(Boss.Width() / 2f), GameConfig.VirtualResolution.X - (int)(Boss.Width() / 2f)),
                    Boss.Game.GameManager.Random.Next((int)(Boss.Height() / 2f) + 100, 500 - (int)(Boss.Height() / 2f))
                );

                Boss.MoveTo(newPosition, 1.5f);
            }

            if (!_targetingPlayer)
            {
                if (_targetingPlayerTimer >= TimeSpan.Zero)
                    _targetingPlayerTimer -= gameTime.ElapsedGameTime;
                else
                {
                    _targetingPlayer = true;
                    Boss.CurrentAnimator.Play("NoAnimation");
                }
            }
            else if (!_stretchingOut)
            {
                Boss.RotateTo(Boss.GetPlayerDirectionAngle());

                if (_stretchingOutTimer >= TimeSpan.Zero)
                    _stretchingOutTimer -= gameTime.ElapsedGameTime;
                else
                {
                    _stretchingOut = true;
                    Boss.CurrentAnimator.Play("StretchOut");
                }
            }
        }
    }
}