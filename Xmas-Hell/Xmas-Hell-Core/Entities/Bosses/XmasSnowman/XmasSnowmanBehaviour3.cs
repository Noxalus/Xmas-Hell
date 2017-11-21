using Microsoft.Xna.Framework;
using XmasHell.FSM;

namespace XmasHell.Entities.Bosses.XmasSnowman
{
    class XmasSnowmanBehaviour2 : AbstractBossBehaviour
    {
        private enum BehaviourState
        {
            TargetingInitialPosition,
            RemovingArms,
            BigArmsAttack
        };

        private readonly FSM<BehaviourState> _stateMachine;

        public XmasSnowmanBehaviour2(Boss boss) : base(boss)
        {
            // State machine
            _stateMachine = new FSM<BehaviourState>("xmas-snowman-behaviour3");

            var targetingInitialPositionBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.TargetingInitialPosition)
                    .OnUpdate(TargetingInitialPositionTaskUpdate);

            var bigArmsAttackBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.BigArmsAttack)
                    .OnEnter(BigArmsAttackTaskEnter)
                    .OnUpdate(BigArmsAttackTaskUpdate);

            _stateMachine.Add(BehaviourState.TargetingInitialPosition, targetingInitialPositionBehaviour);
            _stateMachine.Add(BehaviourState.BigArmsAttack, bigArmsAttackBehaviour);
        }

        #region Tasks

        private void TargetingInitialPositionTaskUpdate(FSMStateData<BehaviourState> data)
        {
            if (!Boss.TargetingPosition)
            {
                Boss.CurrentAnimator.Play("RemoveArms");
                _stateMachine.CurrentState = BehaviourState.RemovingArms;
            }
        }

        private void BigArmsAttackTaskEnter()
        {
            Boss.CurrentAnimator.Play("IdleNoArm");
            ShootPattern();
        }

        private void BigArmsAttackTaskUpdate(FSMStateData<BehaviourState> data)
        {
            if (!Boss.TargetingPosition)
            {
                var newPosition = new Vector2(
                    Boss.Game.GameManager.Random.Next((int)(Boss.Width() / 2f), GameConfig.VirtualResolution.X - (int)(Boss.Width() / 2f)),
                    Boss.Game.GameManager.Random.Next((int)(Boss.Height() / 2f) + 150, 500 - (int)(Boss.Height() / 2f))
                );

                Boss.MoveTo(newPosition, 1.5f);
            }
        }

        #endregion

        #region Animations

        private void AnimationFinishedHandler(string animationName)
        {
            switch (animationName)
            {
                case "RemoveArms":
                    _stateMachine.CurrentState = BehaviourState.BigArmsAttack;
                    break;
            }
        }

        #endregion

        public override void Start()
        {
            Boss.PhysicsWorld.Gravity = GameConfig.DefaultGravity;
            Boss.PhysicsEnabled = true;

            base.Start();

            Boss.Speed = GameConfig.BossDefaultSpeed * 2.5f;

            // Animations
            Boss.CurrentAnimator.AnimationFinished += AnimationFinishedHandler;
            Boss.CurrentAnimator.Play("Idle");

            _stateMachine.CurrentState = BehaviourState.TargetingInitialPosition;

            Boss.MoveToInitialPosition();
        }

        public override void Stop()
        {
            base.Stop();
            Boss.CurrentAnimator.AnimationFinished -= AnimationFinishedHandler;

            Boss.StartShootTimer = false;
            Boss.ShootTimerFinished -= ShootTimerFinished;
            Boss.TargetingPosition = false;
        }

        private void ShootPattern()
        {
            Boss.StartShootTimer = true;
            Boss.ShootTimerTime = 0.3f;
            Boss.ShootTimerFinished += ShootTimerFinished;
        }

        private void ShootTimerFinished(object sender, float e)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _stateMachine.Update(gameTime);
        }
    }
}