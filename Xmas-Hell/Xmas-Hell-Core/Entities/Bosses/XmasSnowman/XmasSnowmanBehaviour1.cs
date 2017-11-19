using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Timers;
using XmasHell.BulletML;
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

        private FSM<BehaviourState> _stateMachine;
        private CountdownTimer _shootBulletTimer;

        public XmasSnowmanBehaviour1(Boss boss) : base(boss)
        {
            InitialBehaviourLife = GameConfig.BossDefaultBehaviourLife * 0.5f;

            _stateMachine = new FSM<BehaviourState>("xmas-snowman-behaviour1");

            var targetingInitialPositionBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.TargetingInitialPosition)
                .OnEnter(TargetingInitialPositionTaskEnter)
                .OnUpdate(TargetingInitialPositionTaskUpdate)
                .OnExit(TargetingInitialPositionTaskExit);

            _stateMachine.Add(BehaviourState.TargetingInitialPosition, targetingInitialPositionBehaviour);
        }

#region Tasks
        private void TargetingInitialPositionTaskEnter()
        {
        }

        private void TargetingInitialPositionTaskUpdate(FSMStateData<BehaviourState> data)
        {
            _shootBulletTimer.Update(data.GameTime);

            var newPosition = new Vector2(
                Boss.Game.GameManager.Random.Next((int)(Boss.Width() / 2f), GameConfig.VirtualResolution.X - (int)(Boss.Width() / 2f)),
                Boss.Game.GameManager.Random.Next((int)(Boss.Height() / 2f) + 200, (int)(Boss.Height() / 2f) + 300)
            );

            Boss.MoveTo(newPosition, 1.5f);
        }

        private void TargetingInitialPositionTaskExit()
        {
        }
#endregion

        public override void Start()
        {
            base.Start();

            Boss.Speed = GameConfig.BossDefaultSpeed * 2.5f;
            _shootBulletTimer = new CountdownTimer(1);

            _shootBulletTimer.Completed += (sender, args) =>
            {
                Boss.Game.GameManager.MoverManager.TriggerPattern("XmasSnowman/pattern1", BulletType.Type2, false, Boss.Position());
                _shootBulletTimer.Restart();
            };

            Boss.CurrentAnimator.Play("Idle");

            _stateMachine.CurrentState = BehaviourState.TargetingInitialPosition;
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _stateMachine.Update(gameTime);
        }
    }
}