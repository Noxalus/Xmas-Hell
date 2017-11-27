using Microsoft.Xna.Framework;
using XmasHell.BulletML;
using XmasHell.FSM;
using XmasHell.Spriter;

namespace XmasHell.Entities.Bosses.XmasSnowman
{
    class XmasSnowmanBehaviour3 : AbstractBossBehaviour
    {
        private enum BehaviourState
        {
            TargetingInitialPosition,
            RemovingCarrot,
            CarrotShot
        };

        private readonly FSM<BehaviourState> _stateMachine;
        private Mover _carrot;

        public XmasSnowmanBehaviour3(Boss boss) : base(boss)
        {
            // State machine
            _stateMachine = new FSM<BehaviourState>("xmas-snowman-behaviour3");

            var targetingInitialPositionBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.TargetingInitialPosition)
                    .OnUpdate(TargetingInitialPositionTaskUpdate);

            var carrotShotBehaviour =
                new FSMBehaviour<BehaviourState>(BehaviourState.CarrotShot)
                    .OnEnter(CarrotShotTaskEnter)
                    .OnUpdate(CarrotShotTaskUpdate);

            _stateMachine.Add(BehaviourState.TargetingInitialPosition, targetingInitialPositionBehaviour);
            _stateMachine.Add(BehaviourState.CarrotShot, carrotShotBehaviour);
        }

        #region Tasks

        private void TargetingInitialPositionTaskUpdate(FSMStateData<BehaviourState> data)
        {
            if (!Boss.TargetingPosition)
            {
                Boss.CurrentAnimator.Play("RemoveCarrot");
                _stateMachine.CurrentState = BehaviourState.RemovingCarrot;
            }
        }

        private void CarrotShotTaskEnter()
        {
            Boss.CurrentAnimator.Play("IdleNoCarrot");
            ShootCarrot();

            Boss.EnableRandomPosition(true);
        }

        private void CarrotShotTaskUpdate(FSMStateData<BehaviourState> data)
        {
            if (_carrot != null)
                return;

            var bossBullets = Boss.Game.GameManager.GetBossBullets();

            if (_carrot == null && bossBullets.Count >= 1 && !bossBullets[0].TopBullet)
                _carrot = bossBullets[0];
        }

        #endregion

        #region Animations

        private void AnimationFinishedHandler(string animationName)
        {
            switch (animationName)
            {
                case "RemoveCarrot":
                    _stateMachine.CurrentState = BehaviourState.CarrotShot;
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
            _carrot = null;
        }

        private void ShootCarrot()
        {
            var carrotPosition = SpriterUtils.GetWorldPosition("nose.png", Boss.CurrentAnimator);
            Boss.TriggerPattern("XmasSnowman/pattern3_2", BulletType.Type2, true, carrotPosition);

            Boss.StartShootTimer = true;
            Boss.ShootTimerTime = 0.003f;
            Boss.ShootTimerFinished += ShootTimerFinished;
        }

        private void ShootTimerFinished(object sender, float e)
        {
            if (_carrot != null)
                Boss.TriggerPattern("XmasSnowman/pattern3_1", BulletType.Type2, false, new Vector2(_carrot.X, _carrot.Y));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _stateMachine.Update(gameTime);
        }
    }
}