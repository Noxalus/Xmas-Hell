using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XmasHell.FSM;

namespace XmasHell.Entities.Bosses.XmasSnowman
{
    class XmasSnowmanBehaviour4 : AbstractBossBehaviour
    {
        private enum BehaviourState
        {
            TargetingInitialPosition,
            RemovingHat,
            HatAttack
        };

        private readonly FSM<BehaviourState> _stateMachine;
        private Hat _hat;

        public XmasSnowmanBehaviour4(Boss boss) : base(boss)
        {
            // State machine
            _stateMachine = new FSM<BehaviourState>("xmas-snowman-behaviour4");

            var targetingInitialPositionBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.TargetingInitialPosition)
                    .OnUpdate(TargetingInitialPositionTaskUpdate);

            var hatAttackBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.HatAttack)
                    .OnEnter(HatAttackTaskEnter)
                    .OnUpdate(HatAttackTaskUpdate);

            _stateMachine.Add(BehaviourState.TargetingInitialPosition, targetingInitialPositionBehaviour);
            _stateMachine.Add(BehaviourState.HatAttack, hatAttackBehaviour);
        }

        #region Tasks

        private void TargetingInitialPositionTaskUpdate(FSMStateData<BehaviourState> data)
        {
            if (!Boss.TargetingPosition)
            {
                Boss.CurrentAnimator.Play("RemoveHat");
                _stateMachine.CurrentState = BehaviourState.RemovingHat;
            }
        }

        private void HatAttackTaskEnter()
        {
            Boss.CurrentAnimator.Play("IdleNoHat");

            var xmasSnowmanBoss = (XmasSnowman) Boss;
            _hat = new Hat(xmasSnowmanBoss, xmasSnowmanBoss.HatAnimator, new Vector2(Boss.Position().X, 100));

            Boss.EnableRandomPosition(true);
        }

        private void HatAttackTaskUpdate(FSMStateData<BehaviourState> data)
        {
        }

        #endregion

        #region Animations

        private void AnimationFinishedHandler(string animationName)
        {
            switch (animationName)
            {
                case "RemoveHat":
                    _stateMachine.CurrentState = BehaviourState.HatAttack;
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
            Boss.CurrentAnimator.AnimationFinished -= AnimationFinishedHandler;
            _hat?.Dispose();
            _hat = null;
            Boss.EnableRandomPosition(false);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _stateMachine.Update(gameTime);

            _hat?.Update(gameTime);
        }

        public override void DrawAfter(SpriteBatch spriteBatch)
        {
            _hat?.Draw(spriteBatch);
        }
    }
}