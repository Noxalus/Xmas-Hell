using BulletML;
using Microsoft.Xna.Framework;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasBell
{
    class XmasBell : Boss
    {
        public XmasBell(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasBell, playerPositionDelegate)
        {
            // Spriter
            SpriterFilename = "Graphics/Sprites/Bosses/XmasBell/xmas-bell";

            // BulletML
            BulletPatternFiles.Add("XmasBell/pattern1");
            BulletPatternFiles.Add("XmasBell/pattern2");
            BulletPatternFiles.Add("XmasBell/pattern4");
            BulletPatternFiles.Add("XmasBell/pattern5");

            // Behaviours
            Behaviours.Add(new XmasBellBehaviour1(this));
            Behaviours.Add(new XmasBellBehaviour2(this));
            //Behaviours.Add(new XmasBellBehaviour3(this));
            Behaviours.Add(new XmasBellBehaviour4(this));
            Behaviours.Add(new XmasBellBehaviour5(this));
        }

        public override void Reset()
        {
            base.Reset();

            RandomMovingArea = new Rectangle(150, 225, Game.ViewportAdapter.VirtualWidth - 150, 225);
        }

        protected override void InitializePhysics(bool setupPhysicsWorld = false)
        {
            base.InitializePhysics();

            // Setup the physics
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(0f, 10f), 0.90f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(this, "clapper.png"));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "clapper-ball.png"));
        }

        protected override void UpdateBehaviourIndex()
        {
            base.UpdateBehaviourIndex();
        }
    }
}