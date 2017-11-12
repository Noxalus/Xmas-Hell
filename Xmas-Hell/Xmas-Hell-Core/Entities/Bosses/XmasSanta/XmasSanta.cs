using BulletML;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasSanta
{
    class XmasSanta : Boss
    {
        public XmasSanta(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasSanta, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("XmasSanta/pattern1");

            // Behaviours
            Behaviours.Add(new XmasSantaBehaviour1(this));

            SpriterFilename = "Graphics/Sprites/Bosses/XmasSanta/xmas-santa";
        }

        protected override void LoadSpriterSprite()
        {
            base.LoadSpriterSprite();
        }

        protected override void InitializePhysics(bool setupPhysicsWorld = false)
        {
            base.InitializePhysics();

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png"));
        }

        protected override void UpdateBehaviourIndex()
        {
            base.UpdateBehaviourIndex();
        }
    }
}