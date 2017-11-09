using BulletML;
using Microsoft.Xna.Framework;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandy : Boss
    {
        public XmasCandy(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasCandy, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("XmasCandy/pattern1");
            BulletPatternFiles.Add("XmasCandy/pattern2");
            BulletPatternFiles.Add("XmasCandy/pattern3");
            BulletPatternFiles.Add("XmasCandy/pattern4");

            // Behaviours
            Behaviours.Add(new XmasCandyBehaviour1(this));
            Behaviours.Add(new XmasCandyBehaviour2(this));
            Behaviours.Add(new XmasCandyBehaviour3(this));
            Behaviours.Add(new XmasCandyBehaviour4(this));

            SpriterFilename = "Graphics/Sprites/Bosses/XmasCandy/xmas-candy";
        }

        protected override void LoadSpriterSprite()
        {
            base.LoadSpriterSprite();
            CurrentAnimator.StretchOut = false;
        }

        public override void Reset()
        {
            base.Reset();

            // For the 4th pattern, the initial bounding boxes are removed
            // We need to make sure we re-add them properly
            Game.GameManager.CollisionWorld.ClearBossHitboxes();
            InitializePhysics();

            // Same thing for stretch out behaviour, disabled for the 4th pattern
            CurrentAnimator.StretchOut = false;
        }

        protected override void InitializePhysics()
        {
            base.InitializePhysics();

            // Top part
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(-75, 20), 0.3f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(-65, -20), 0.3f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(-20, -50), 0.3f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(20, -50), 0.3f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(65, -20), 0.3f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(75, 20), 0.3f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(75, 60), 0.3f));

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(this, "body2.png"));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body3.png"));
        }

        protected override void UpdateBehaviourIndex()
        {
            base.UpdateBehaviourIndex();
        }
    }
}