using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Timers;
using XmasHell.BulletML;
using XmasHell.Spriter;

namespace XmasHell.Entities.Bosses.XmasSnowflake
{
    class XmasSnowflakeBehaviour4 : AbstractBossBehaviour
    {
        private CountdownTimer _shootBulletTimer;
        private List<ActionPoint> _actionPoints;

        public XmasSnowflakeBehaviour4(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            //Boss.Speed = 500f;
            Boss.CurrentAnimator.Play("Idle");

            _shootBulletTimer = new CountdownTimer(0.2);

            _shootBulletTimer.Completed += (sender, args) =>
            {
                ShootBulletFromBranches();

                _shootBulletTimer.Restart();
            };

            _actionPoints = new List<ActionPoint>(8);

            for (int i = 0; i < 8; i++)
                _actionPoints.Add(new ActionPoint());
        }

        public override void Stop()
        {
            base.Stop();
        }

        private void ShootBulletFromBranches()
        {
            foreach (var actionPoint in _actionPoints)
            {
                Boss.Game.GameManager.MoverManager.TriggerPattern(
                    "XmasSnowflake/pattern4",
                    BulletType.Type2,
                    false,
                    actionPoint.Position,
                    actionPoint.Direction
                );
            }
        }

        private void UpdateActionPointPosition()
        {
            for (int i = 0; i < 8; i++)
            {
                if (Boss.CurrentAnimator.FrameData != null && Boss.CurrentAnimator.FrameData.PointData.ContainsKey("action_point_" + (i + 1)))
                {
                    var actionPoint = Boss.CurrentAnimator.FrameData.PointData["action_point_" + (i + 1)];
                    _actionPoints[i].Position = Boss.Position() + new Vector2(actionPoint.X, -actionPoint.Y);
                    _actionPoints[i].Direction = actionPoint.Angle;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            var newPosition = new Vector2(
                Boss.Game.GameManager.Random.Next((int)(Boss.Width() / 2f), GameConfig.VirtualResolution.X - (int)(Boss.Width() / 2f)),
                Boss.Game.GameManager.Random.Next((int)(Boss.Height() / 2f) + 42, GameConfig.VirtualResolution.Y - (int)(Boss.Height() / 2f))
            );

            Boss.MoveTo(newPosition, 3f);

            UpdateActionPointPosition();
            _shootBulletTimer.Update(gameTime);

            base.Update(gameTime);
        }
    }
}