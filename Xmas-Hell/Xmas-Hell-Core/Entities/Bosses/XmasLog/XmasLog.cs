using BulletML;
using Microsoft.Xna.Framework;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasLog
{
    class XmasLog : Boss
    {
        public XmasLog(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasLog, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("XmasLog/pattern1");

            // Spriter
            SpriterFilename = "Graphics/Sprites/Bosses/XmasLog/xmas-log";

            // Behaviours
            Behaviours.Add(new XmasLogBehaviour1(this));

            InitialPosition = new Vector2(GameConfig.VirtualResolution.X / 2f, GameConfig.VirtualResolution.Y * 0.15f);
        }

        protected override void InitializePhysics(bool setupPhysicsWorld = false)
        {
            base.InitializePhysics(setupPhysicsWorld);

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(this, "body.png"));
        }
    }
}