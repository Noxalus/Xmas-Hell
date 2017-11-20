using System.Linq;
using BulletML;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;

namespace XmasHell.Entities.Bosses.XmasSnowman
{
    class XmasSnowman : Boss
    {
        public CustomSpriterAnimator SnowballAnimator;

        public XmasSnowman(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasSnowman, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("XmasSnowman/pattern1");
            BulletPatternFiles.Add("XmasSnowman/pattern3_1");
            BulletPatternFiles.Add("XmasSnowman/pattern3_2");

            // Behaviours
            Behaviours.Add(new XmasSnowmanBehaviour1(this));
            Behaviours.Add(new XmasSnowmanBehaviour3(this));

            SpriterFilename = "Graphics/Sprites/Bosses/XmasSnowman/xmas-snowman";
        }

        protected override void LoadSpriterSprite()
        {
            base.LoadSpriterSprite();

            SnowballAnimator = Animators.First(a => a.Entity != null && a.Entity.Name == "Snowball");
        }

        protected override void InitializePhysics(bool setupPhysicsWorld = false)
        {
            base.InitializePhysics(true);

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png"));
        }

        protected override void UpdateBehaviourIndex()
        {
            base.UpdateBehaviourIndex();

            CurrentBehaviourIndex = 1;
        }
    }
}