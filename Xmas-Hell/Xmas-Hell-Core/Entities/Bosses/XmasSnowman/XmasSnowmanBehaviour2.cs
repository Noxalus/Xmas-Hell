using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private BigArms _leftArm;
        private BigArms _rightArm;

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

            var xmasSnowmanBoss = (XmasSnowman)Boss;
            _leftArm = new BigArms(xmasSnowmanBoss, xmasSnowmanBoss.BigArmsAnimator, false);
            _rightArm = new BigArms(xmasSnowmanBoss, xmasSnowmanBoss.BigArmsAnimator, true);

            Boss.EnableRandomPosition(true);
        }

        private void BigArmsAttackTaskUpdate(FSMStateData<BehaviourState> data)
        {
            _leftArm?.Update(data.GameTime);
            _rightArm?.Update(data.GameTime);
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

            Boss.EnableRandomPosition(false);

            Boss.CurrentAnimator.AnimationFinished -= AnimationFinishedHandler;

            Boss.StartShootTimer = false;
            Boss.ShootTimerFinished -= ShootTimerFinished;
            Boss.TargetingPosition = false;

            _leftArm?.Dispose();
            _rightArm?.Dispose();

            _leftArm = null;
            _rightArm = null;
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

        public override void DrawAfter(SpriteBatch spriteBatch)
        {
            base.DrawAfter(spriteBatch);

            _leftArm?.Draw(spriteBatch);
            _rightArm?.Draw(spriteBatch);
        }
    }
}