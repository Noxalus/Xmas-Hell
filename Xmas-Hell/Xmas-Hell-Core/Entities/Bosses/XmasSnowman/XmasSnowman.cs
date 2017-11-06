using BulletML;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasSnowman
{
    class XmasSnowman : Boss
    {
        public XmasSnowman(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasSnowman, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("XmasSnowman/pattern1");

            // Behaviours
            Behaviours.Add(new XmasSnowmanBehaviour1(this));

            SpriterFilename = "Graphics/Sprites/Bosses/XmasSnowman/xmas-snowman";
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