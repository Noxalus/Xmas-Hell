using BulletML;
using Microsoft.Xna.Framework;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasBell
{
    class XmasBell : Boss
    {
        public XmasBell(XmasHell game, PositionDelegate playerPositionDelegate) : base(game, playerPositionDelegate)
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
            Behaviours.Add(new XmasBellBehaviour3(this));
            Behaviours.Add(new XmasBellBehaviour4(this));
            Behaviours.Add(new XmasBellBehaviour5(this));
        }

        protected override void InitializePhysics()
        {
            base.InitializePhysics();

            // Setup the physics
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(0f, 10f), 0.90f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(this, "clapper.png", Vector2.Zero, 1f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(this, "clapper-ball.png", Vector2.Zero, 1f));
        }

        protected override void UpdateBehaviourIndex()
        {
            base.UpdateBehaviourIndex();

            //CurrentBehaviourIndex = 4;
        }
    }
}