using Microsoft.Xna.Framework;
using MonoGame.Extended;
using XmasHell.FSM;

namespace XmasHell.Entities.Bosses.XmasLog
{
    class XmasLogBehaviour2 : AbstractBossBehaviour
    {
        private enum BehaviourState
        {
            TargetingScreenCenter,
            HelicopterPattern
        };

        private readonly FSM<BehaviourState> _stateMachine;

        public XmasLogBehaviour2(Boss boss) : base(boss)
        {
            // State machine
            _stateMachine = new FSM<BehaviourState>("xmas-log-behaviour2");

            var targetingScreenCenterBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.TargetingScreenCenter)
                    .OnUpdate(TargetingScreenCenterTaskUpdate);

            var shootingPatternBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.HelicopterPattern)
                    .OnEnter(HelicopterPatternTaskEnter)
                    .OnUpdate(HelicopterPatternTaskUpdate);

            _stateMachine.Add(BehaviourState.TargetingScreenCenter, targetingScreenCenterBehaviour);
            _stateMachine.Add(BehaviourState.HelicopterPattern, shootingPatternBehaviour);
        }

        private void ShootTimerFinished(object sender, float e)
        {
        }

        private void TargetingScreenCenterTaskUpdate(FSMStateData<BehaviourState> data)
        {
            if (!Boss.TargetingPosition && Boss.Position().EqualsWithTolerence(Boss.Game.ViewportAdapter.Center.ToVector2()))
                _stateMachine.CurrentState = BehaviourState.HelicopterPattern;
        }

        private void HelicopterPatternTaskEnter()
        {
            Boss.CurrentAnimator.Play("Whirligig");
            Boss.CurrentAnimator.Speed = 0.2f;

            Boss.ShootTimerTime = 0.05f;
            Boss.StartShootTimer = true;
        }

        private void HelicopterPatternTaskUpdate(FSMStateData<BehaviourState> data)
        {
            // Shoot bullet?
        }

        public override void Start()
        {
            base.Start();

            Boss.CurrentAnimator.Play("Idle");
            Boss.MoveToCenter();
            Boss.ShootTimerFinished += ShootTimerFinished;

            Boss.CurrentAnimator.AnimationFinished += AnimationFinished;

            _stateMachine.CurrentState = BehaviourState.TargetingScreenCenter;
        }

        private void AnimationFinished(string animationName)
        {
            switch (animationName)
            {
                case "WhirligigExpandVertical":
                    Boss.CurrentAnimator.Play("Whirligig");
                    break;
                case "WhirligigExpandHorizontal":
                    Boss.CurrentAnimator.Play("Whirligig");
                    break;
                case "Whirligig":

                    Boss.CurrentAnimator.Play("WhirligigExpandVertical");

                    //Boss.CurrentAnimator.Play(Boss.Game.GameManager.Random.NextDouble() > 0.5f
                    //    ? "WhirligigExpandVertical"
                    //    : "WhirligigExpandHorizontal");
                    break;
            }
        }

        public override void Stop()
        {
            base.Stop();

            Boss.StartShootTimer = false;
            Boss.ShootTimerFinished -= ShootTimerFinished;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _stateMachine.Update(gameTime);
        }
    }
}
