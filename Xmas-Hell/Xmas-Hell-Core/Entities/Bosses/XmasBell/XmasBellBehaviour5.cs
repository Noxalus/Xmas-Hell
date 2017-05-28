using System;
using Microsoft.Xna.Framework;

namespace XmasHell.Entities.Bosses.XmasBell
{
    class XmasBellBehaviour5 : AbstractBossBehaviour
    {
        public XmasBellBehaviour5(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.MoveToInitialPosition();

            // Events
            Boss.CurrentAnimator.AnimationFinished += AnimationFinished;
            Boss.CurrentAnimator.EventTriggered += AnimationEventTriggered; ;
        }

        private void AnimationEventTriggered(string name)
        {
            if (name == "hit-bottom")
            {
                Boss.Game.Camera.Shake(0.5f, 20);
            }
            else if (name == "hit-wall")
            {
                Boss.Game.Camera.Shake(0.25f, 10);

                var randomBulletCount = Boss.Game.GameManager.Random.Next(1, 5);
                var bounds = new Rectangle(
                    50, -10,
                    Boss.Game.ViewportAdapter.VirtualWidth - 50, 10
                );

                for (int i = 0; i < randomBulletCount; i++)
                {
                    var patternPosition = Boss.Game.GameManager.GetRandomPosition(false, bounds);
                    Boss.TriggerPattern("XmasBell/pattern5", BulletML.BulletType.Type4, false, patternPosition);
                }

            }
        }

        private void AnimationFinished(string name)
        {
            if (name == "Clapper")
            {
                Boss.CurrentAnimator.Play("ClapperBalance");
            }
        }

        public override void Stop()
        {
            base.Stop();

            Boss.CurrentAnimator.AnimationFinished -= AnimationFinished;
        }

        private void StartClapperPattern()
        {
            Boss.CurrentAnimator.Play("Clapper");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Boss.CurrentAnimator.CurrentAnimation.Name == "Idle" && Boss.Position() == Boss.InitialPosition)
                StartClapperPattern();
        }
    }
}