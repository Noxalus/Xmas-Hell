using BulletML;
using XmasHell.Physics.Collision;
using XmasHell.Entities.Bosses.XmasLog;

namespace XmasHell.Entities.Bosses.XmasLog
{
    class XmasLog : Boss
    {
        public XmasLog(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasLog, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("sample");

            // Spriter
            SpriterFilename = "Graphics/Sprites/Bosses/XmasLog/xmas-log";

            // Behaviours
            Behaviours.Add(new XmasLogBehaviour1(this));
        }

        protected override void InitializePhysics()
        {
            base.InitializePhysics();

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(this, "body.png"));
        }
    }
}