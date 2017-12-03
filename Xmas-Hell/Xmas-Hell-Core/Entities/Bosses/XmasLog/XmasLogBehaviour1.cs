using Microsoft.Xna.Framework;
using XmasHell.BulletML;
using XmasHell.FSM;

namespace XmasHell.Entities.Bosses.XmasLog
{
    class XmasLogBehaviour1 : AbstractBossBehaviour
    {
        private enum BehaviourState
        {
            TargetingInitialPosition,
            ShootingPattern
        };

        private readonly FSM<BehaviourState> _stateMachine;

        public XmasLogBehaviour1(Boss boss) : base(boss)
        {
            // State machine
            _stateMachine = new FSM<BehaviourState>("xmas-log-behaviour1");

            var targetingInitialPositionBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.TargetingInitialPosition)
                    .OnUpdate(TargetingInitialPositionTaskUpdate);

            var shootingPatternBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.ShootingPattern)
                    .OnEnter(ShootingPatternTaskEnter)
                    .OnUpdate(ShootingPatternTaskUpdate);

            _stateMachine.Add(BehaviourState.TargetingInitialPosition, targetingInitialPositionBehaviour);
            _stateMachine.Add(BehaviourState.ShootingPattern, shootingPatternBehaviour);

        }

        private void ShootTimerFinished(object sender, float e)
        {
            Boss.TriggerPattern("XmasLog/pattern1", BulletType.Type2, false, Boss.Position());
        }

        private void TargetingInitialPositionTaskUpdate(FSMStateData<BehaviourState> data)
        {
            if (!Boss.TargetingPosition)
                _stateMachine.CurrentState = BehaviourState.ShootingPattern;
        }

        private void ShootingPatternTaskEnter()
        {
            Boss.ShootTimerTime = 0.05f;
            Boss.StartShootTimer = true;
        }

        private void ShootingPatternTaskUpdate(FSMStateData<BehaviourState> data)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.CurrentAnimator.Play("Idle");
            Boss.MoveToInitialPosition();
            Boss.ShootTimerFinished += ShootTimerFinished;

            _stateMachine.CurrentState = BehaviourState.TargetingInitialPosition;
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
