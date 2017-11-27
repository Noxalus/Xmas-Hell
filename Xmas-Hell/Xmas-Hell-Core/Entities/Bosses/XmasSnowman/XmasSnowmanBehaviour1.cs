using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XmasHell.FSM;

namespace XmasHell.Entities.Bosses.XmasSnowman
{
    class XmasSnowmanBehaviour1 : AbstractBossBehaviour
    {
        private enum BehaviourState
        {
            TargetingInitialPosition,
            StartInvokingSnowball,
            InvokingSnowball,
            StopInvokingSnowball,
            ThrowingSnowball
        };

        private readonly FSM<BehaviourState> _stateMachine;
        private readonly Vector2 _initialBehaviourPosition;
        private readonly List<Snowball> _snowballs = new List<Snowball>();
        private Snowball _currentSnowball;

        public XmasSnowmanBehaviour1(Boss boss) : base(boss)
        {
            InitialBehaviourLife = GameConfig.BossDefaultBehaviourLife * 0.5f;

            // State machine
            _stateMachine = new FSM<BehaviourState>("xmas-snowman-behaviour1");

            var targetingInitialPositionBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.TargetingInitialPosition)
                .OnEnter(TargetingInitialPositionTaskEnter)
                .OnUpdate(TargetingInitialPositionTaskUpdate)
                .OnExit(TargetingInitialPositionTaskExit);

            _stateMachine.Add(BehaviourState.TargetingInitialPosition, targetingInitialPositionBehaviour);

            _initialBehaviourPosition = new Vector2(Boss.InitialPosition.X, Boss.InitialPosition.Y + 500);
        }

        #region Tasks

        private void TargetingInitialPositionTaskEnter()
        {
        }

        private void TargetingInitialPositionTaskUpdate(FSMStateData<BehaviourState> data)
        {
            if (!Boss.TargetingPosition)
            {
                Boss.CurrentAnimator.Play("StartInvokeSnowball");
                _stateMachine.CurrentState = BehaviourState.StartInvokingSnowball;
            }
        }

        private void TargetingInitialPositionTaskExit()
        {
        }

        #endregion

        #region Animations

        private void AnimationFinishedHandler(string animationName)
        {
            switch (animationName)
            {
                case "StartInvokeSnowball":
                    InvokeSnowball();
                break;
                case "StopInvokeSnowball":
                    ThrowCurrentSnowball();
                break;
                case "ThrowSnowball":
                    // TODO: Add timer for the new snowball
                    Boss.CurrentAnimator.Play("StartInvokeSnowball");
                    _stateMachine.CurrentState = BehaviourState.ThrowingSnowball;
                break;
            }
        }

        #endregion

        private void InvokeSnowball()
        {
            Boss.CurrentAnimator.Play("InvokeSnowball");
            _stateMachine.CurrentState = BehaviourState.InvokingSnowball;

            var snowmanBoss = (XmasSnowman) Boss;
            var snowballPosition = Boss.InitialPosition;

            _currentSnowball = new Snowball(snowmanBoss, snowmanBoss.SnowballAnimator, snowballPosition);
            _currentSnowball.SpawnAnimationFinished += SnowballSpawnAnimationFinished;
            _snowballs.Add(_currentSnowball);
        }

        private void SnowballSpawnAnimationFinished(object sender, System.EventArgs e)
        {
            _currentSnowball.SpawnAnimationFinished -= SnowballSpawnAnimationFinished;
            Boss.CurrentAnimator.Play("StopInvokeSnowball");
            _stateMachine.CurrentState = BehaviourState.StopInvokingSnowball;
        }

        private void ThrowCurrentSnowball()
        {
            Boss.CurrentAnimator.Play("ThrowSnowball");
            _stateMachine.CurrentState = BehaviourState.ThrowingSnowball;
            _currentSnowball.ThrowTo(Boss.GetPlayerDirection(), 750f);
        }

        public override void Start()
        {
            base.Start();

            Boss.PhysicsWorld.Gravity = GameConfig.DefaultGravity;
            Boss.PhysicsEnabled = true;

            Boss.Speed = GameConfig.BossDefaultSpeed * 2.5f;

            // Animations
            Boss.CurrentAnimator.AnimationFinished += AnimationFinishedHandler;
            Boss.CurrentAnimator.Play("Idle");

            // State machine
            _stateMachine.CurrentState = BehaviourState.TargetingInitialPosition;

            Boss.MoveTo(_initialBehaviourPosition);
        }

        public override void Stop()
        {
            base.Stop();

            Boss.PhysicsEnabled = false;

            Boss.CurrentAnimator.AnimationFinished -= AnimationFinishedHandler;

            foreach (var snowball in _snowballs)
                snowball.Dispose();

            _snowballs.Clear();
            _currentSnowball = null;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _stateMachine.Update(gameTime);

            foreach (var snowball in _snowballs)
                snowball.Update(gameTime);
        }

        public override void DrawAfter(SpriteBatch spriteBatch)
        {
            foreach (var snowball in _snowballs)
                snowball.Draw(spriteBatch);
        }
    }
}