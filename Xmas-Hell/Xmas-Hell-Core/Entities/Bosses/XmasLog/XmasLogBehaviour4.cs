using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XmasHell.FSM;

namespace XmasHell.Entities.Bosses.XmasLog
{
    class XmasLogBehaviour4 : AbstractBossBehaviour
    {
        private Holly _holly;

        private enum BehaviourState
        {
            TargetingInitialPosition,
            RemovingHolly,
            HollyAttack
        }

        private readonly FSM<BehaviourState> _stateMachine;

        public XmasLogBehaviour4(Boss boss) : base(boss)
        {
            // State machine
            _stateMachine = new FSM<BehaviourState>("xmas-log-behaviour4");

            var targetingInitialPositionBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.TargetingInitialPosition)
                    .OnUpdate(TargetingInitialPositionTaskUpdate);

            var removingHollyBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.HollyAttack)
                    .OnEnter(RemovingHollyTaskEnter);

            var hollyAttackBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.HollyAttack)
                    .OnEnter(HollyAttackTaskEnter)
                    .OnUpdate(HollyAttackTaskUpdate);

            _stateMachine.Add(BehaviourState.TargetingInitialPosition, targetingInitialPositionBehaviour);
            _stateMachine.Add(BehaviourState.RemovingHolly, removingHollyBehaviour);
            _stateMachine.Add(BehaviourState.HollyAttack, hollyAttackBehaviour);
        }

        private void ShootTimerFinished(object sender, float e)
        {
        }

        private void AnimationFinished(string animationName)
        {
            switch (animationName)
            {
                case "RemovingHolly":
                    Boss.CurrentAnimator.Play("Whirligig");
                    _stateMachine.CurrentState = BehaviourState.HollyAttack;
                    break;
            }
        }

        private void TargetingInitialPositionTaskUpdate(FSMStateData<BehaviourState> data)
        {
            if (!Boss.TargetingPosition)
                _stateMachine.CurrentState = BehaviourState.RemovingHolly;
        }

        private void RemovingHollyTaskEnter()
        {
            var xmasLogBoss = (XmasLog)Boss;
            _holly = new Holly(xmasLogBoss, xmasLogBoss.HollyAnimator);
            //Boss.CurrentAnimator.Play("IdleNoHolly");
        }

        private void HollyAttackTaskEnter()
        {

        }

        private void HollyAttackTaskUpdate(FSMStateData<BehaviourState> data)
        {
            _holly?.Update(data.GameTime);
        }

        public override void Start()
        {
            base.Start();

            Boss.CurrentAnimator.Play("Idle");
            Boss.MoveToInitialPosition();
            Boss.ShootTimerFinished += ShootTimerFinished;
            Boss.CurrentAnimator.AnimationFinished += AnimationFinished;

            _stateMachine.CurrentState = BehaviourState.TargetingInitialPosition;
        }

        public override void Stop()
        {
            base.Stop();

            Boss.StartShootTimer = false;
            Boss.ShootTimerFinished -= ShootTimerFinished;
            Boss.CurrentAnimator.AnimationFinished -= AnimationFinished;

            if (_holly != null)
            {
                _holly.Dispose();
                _holly = null;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _stateMachine.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            _holly?.Draw(spriteBatch);
        }
    }
}
