using System;
using Microsoft.Xna.Framework;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandyBehaviour2 : AbstractBossBehaviour
    {
        private bool _targetingPlayer;
        private bool _stretchingOut;
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

            Boss.MoveToInitialPosition(true);
        }

        private void ResetStretchOutAttack()
        {
            Boss.RotateTo(0);
            Boss.CurrentAnimator.Play("Idle");
            _targetingPlayer = false;
            _stretchingOut = false;
            _stretchingOutTimer = TimeSpan.FromSeconds(Boss.Game.GameManager.Random.NextDouble() * 0.75f);
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

            if (!_targetingPlayer && !Boss.TargetingPosition && Boss.Position().Equals(Boss.InitialPosition))
            {
                _targetingPlayer = true;
                Boss.CurrentAnimator.Play("NoAnimation");
            }
            else if (_targetingPlayer && !_stretchingOut)
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

            if (Boss.Game.GameManager.IsOutside(Boss.ActionPointPosition()))
            {
                Boss.Game.Camera.Shake(0.5f, 100f);

                Boss.TriggerPattern(
                    "XmasCandy/pattern2",
                    BulletML.BulletType.Type1, false,
                    Boss.ActionPointPosition(),
                    (float)(Boss.ActionPointDirection() + Math.PI)
                );

                ResetStretchOutAttack();
            }
        }
    }
}