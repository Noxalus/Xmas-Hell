using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using XmasHell.BulletML;
using XmasHell.FSM;

namespace XmasHell.Entities.Bosses.XmasLog
{
    class XmasLogBehaviour3 : AbstractBossBehaviour
    {
        private List<BabyLog> _babyLogs;

        private enum BehaviourState
        {
            TargetingInitialPosition,
            BabyLogs
        };

        private readonly FSM<BehaviourState> _stateMachine;

        public XmasLogBehaviour3(Boss boss) : base(boss)
        {
            // State machine
            _stateMachine = new FSM<BehaviourState>("xmas-log-behaviour3");

            var targetingInitialPositionBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.TargetingInitialPosition)
                    .OnUpdate(TargetingInitialPositionTaskUpdate);

            var babyLogBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.BabyLogs)
                    .OnEnter(BabyLogTaskEnter)
                    .OnUpdate(BabyLogTaskUpdate);

            _stateMachine.Add(BehaviourState.TargetingInitialPosition, targetingInitialPositionBehaviour);
            _stateMachine.Add(BehaviourState.BabyLogs, babyLogBehaviour);

            _babyLogs = new List<BabyLog>();
        }

        private void ShootTimerFinished(object sender, float e)
        {
        }

        private void TargetingInitialPositionTaskUpdate(FSMStateData<BehaviourState> data)
        {
            if (!Boss.TargetingPosition)
                _stateMachine.CurrentState = BehaviourState.BabyLogs;
        }

        private void BabyLogTaskEnter()
        {
        }

        private void BabyLogTaskUpdate(FSMStateData<BehaviourState> data)
        {
            foreach (var babyLog in _babyLogs)
                babyLog.Update(data.GameTime);
        }

        public override void Start()
        {
            base.Start();

            Boss.CurrentAnimator.Play("Idle");
            Boss.MoveToInitialPosition();
            Boss.ShootTimerFinished += ShootTimerFinished;

            _stateMachine.CurrentState = BehaviourState.TargetingInitialPosition;

            var xmasLogBoss = (XmasLog) Boss;
            _babyLogs.Add(new BabyLog(xmasLogBoss, xmasLogBoss.BabyLogAnimator, Boss.Game.GameManager.GetRandomPosition()));
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (var babyLog in _babyLogs)
                babyLog.GetCurrentAnimator().Draw(spriteBatch);
        }
    }
}
