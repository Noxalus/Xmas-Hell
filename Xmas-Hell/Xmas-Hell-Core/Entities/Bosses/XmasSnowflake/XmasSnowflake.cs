using BulletML;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasSnowflake
{
    class XmasSnowflake : Boss
    {
        public XmasSnowflake(XmasHell game, PositionDelegate playerPositionDelegate) : base(game, playerPositionDelegate)
        {
            InitialLife = 500f;

            // BulletML
            BulletPatternFiles.Add("XmasSnowflake/pattern1");
            BulletPatternFiles.Add("XmasSnowflake/pattern2");
            BulletPatternFiles.Add("XmasSnowflake/pattern3");
            BulletPatternFiles.Add("XmasSnowflake/pattern4");

            // Behaviours
            Behaviours.Add(new XmasSnowflakeBehaviour1(this));
            Behaviours.Add(new XmasSnowflakeBehaviour2(this));
            Behaviours.Add(new XmasSnowflakeBehaviour3(this));
            Behaviours.Add(new XmasSnowflakeBehaviour4(this));

            SpriterFilename = "Graphics/Sprites/Bosses/XmasSnowflake/xmas-snowflake";
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

            //CurrentBehaviourIndex = 3;
        }
    }
}