using BulletML;
using Microsoft.Xna.Framework;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandy : Boss
    {
        public XmasCandy(XmasHell game, PositionDelegate playerPositionDelegate) : base(game, playerPositionDelegate)
        {
            InitialLife = 500f;

            // BulletML
            BulletPatternFiles.Add("XmasCandy/pattern1");

            // Behaviours
            Behaviours.Add(new XmasCandyBehaviour1(this));

            SpriterFilename = "Graphics/Sprites/Bosses/XmasCandy/xmas-candy";
        }

        protected override void LoadSpriterSprite()
        {
            base.LoadSpriterSprite();
            CurrentAnimator.StretchOut = false;
        }

        protected override void InitializePhysics()
        {
            base.InitializePhysics();

            // Top part
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(-60, 20), 0.3f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(-55, -20), 0.3f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(-20, -50), 0.3f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(20, -50), 0.3f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(55, -20), 0.3f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(60, 20), 0.3f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(60, 60), 0.3f));

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(this, "body2.png"));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body3.png"));
        }

        protected override void UpdateBehaviourIndex()
        {
            base.UpdateBehaviourIndex();
        }
    }
}