using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XmasHell.BulletML;
using XmasHell.FSM;

namespace XmasHell.Entities.Bosses.XmasSnowman
{
    class XmasSnowmanBehaviour3 : AbstractBossBehaviour
    {
        private enum BehaviourState
        {
        };

        private readonly FSM<BehaviourState> _stateMachine;
        private Mover _carrot;

        public XmasSnowmanBehaviour3(Boss boss) : base(boss)
        {
            // State machine
            _stateMachine = new FSM<BehaviourState>("xmas-snowman-behaviour3");
        }

        #region Animations

        private void AnimationFinishedHandler(string animationName)
        {
            switch (animationName)
            {
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

            Boss.MoveToInitialPosition();

            ShootCarrot();
        }

        public override void Stop()
        {
            base.Stop();
            Boss.CurrentAnimator.AnimationFinished -= AnimationFinishedHandler;

            Boss.StartShootTimer = false;
            Boss.ShootTimerFinished -= ShootTimerFinished;
            Boss.TargetingPosition = false;
            _carrot = null;
        }

        private void ShootCarrot()
        {
            Boss.TriggerPattern("XmasSnowman/pattern3_2", BulletType.Type2, true, Boss.ActionPointPosition());

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

            var bossBullets = Boss.Game.GameManager.GetBossBullets();

            if (_carrot == null && bossBullets.Count >= 1 && !bossBullets[0].TopBullet)
                _carrot = bossBullets[0];

            _stateMachine.Update(gameTime);
        }
    }
}