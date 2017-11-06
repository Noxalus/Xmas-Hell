using BulletML;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasTree
{
    class XmasTree : Boss
    {
        public XmasTree(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasTree, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("XmasTree/pattern1");

            // Behaviours
            Behaviours.Add(new XmasTreeBehaviour1(this));

            SpriterFilename = "Graphics/Sprites/Bosses/XmasTree/xmas-tree";
        }

        protected override void LoadSpriterSprite()
        {
            base.LoadSpriterSprite();
        }

        protected override void InitializePhysics()
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