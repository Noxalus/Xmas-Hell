using BulletML;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasReindeer
{
    class XmasReindeer : Boss
    {
        public XmasReindeer(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasReindeer, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("XmasReindeer/pattern1");

            // Behaviours
            Behaviours.Add(new XmasReindeerBehaviour1(this));

            SpriterFilename = "Graphics/Sprites/Bosses/XmasReindeer/xmas-reindeer";
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