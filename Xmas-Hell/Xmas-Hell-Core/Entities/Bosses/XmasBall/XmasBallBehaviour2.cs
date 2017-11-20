using Microsoft.Xna.Framework;
using XmasHell.BulletML;

namespace XmasHell.Entities.Bosses.XmasBall
{
    class XmasBallBehaviour2 : AbstractBossBehaviour
    {
        private Vector2 _screenCenter;
        private bool _patternShot;

        public XmasBallBehaviour2(Boss boss) : base(boss)
        {
            InitialBehaviourLife = GameConfig.BossDefaultBehaviourLife * 2f;

            _screenCenter = new Vector2(
                GameConfig.VirtualResolution.X / 2f,
                GameConfig.VirtualResolution.Y / 2f
            );
        }

        public override void Start()
        {
            base.Start();

            Boss.CurrentAnimator.Play("Idle");
            _patternShot = false;

            Boss.Invincible = true;
            Boss.MoveTo(_screenCenter, 2f, true);

            Boss.CurrentAnimator.AnimationFinished += AnimationFinishedHandler;
        }

        public override void Stop()
        {
            base.Stop();

            Boss.CurrentAnimator.AnimationFinished -= AnimationFinishedHandler;
        }

        private void AnimationFinishedHandler(string animationName)
        {
            switch (animationName)
            {
                case "Breathe_In":
                    Boss.CurrentAnimator.Play("Big_Idle");
                    break;
                case "Breathe_Out":
                    Boss.CurrentAnimator.Play("Idle");
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Boss.CurrentAnimator.Position.Equals(_screenCenter))
            {
                if (Boss.Invincible)
                    Boss.Invincible = false;

                if (Boss.CurrentAnimator.CurrentAnimation.Name == "Idle")
                    Boss.CurrentAnimator.Play("Breathe_In");
                else if (Boss.CurrentAnimator.CurrentAnimation.Name == "Big_Idle")
                {
                    Boss.CurrentAnimator.Play("Breathe_Out");
                }
                else if (Boss.CurrentAnimator.CurrentAnimation.Name == "Breathe_Out")
                {
                    if (!_patternShot)
                    {
                        Boss.Game.GameManager.MoverManager.TriggerPattern("XmasBall/pattern2", BulletType.Type3, false, Boss.Position());
                        _patternShot = true;
                    }
                }
            }
        }
    }
}