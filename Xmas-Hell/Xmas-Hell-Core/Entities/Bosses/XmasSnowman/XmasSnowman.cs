using System.Linq;
using BulletML;
using Microsoft.Xna.Framework;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;

namespace XmasHell.Entities.Bosses.XmasSnowman
{
    class XmasSnowman : Boss
    {
        public CustomSpriterAnimator SnowballAnimator;
        public CustomSpriterAnimator BigArmsAnimator;
        public CustomSpriterAnimator HatAnimator;

        public XmasSnowman(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasSnowman, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("XmasSnowman/pattern1");
            BulletPatternFiles.Add("XmasSnowman/pattern3_1");
            BulletPatternFiles.Add("XmasSnowman/pattern3_2");

            // Behaviours
            Behaviours.Add(new XmasSnowmanBehaviour1(this));
            Behaviours.Add(new XmasSnowmanBehaviour2(this));
            Behaviours.Add(new XmasSnowmanBehaviour3(this));
            Behaviours.Add(new XmasSnowmanBehaviour4(this));

            SpriterFilename = "Graphics/Sprites/Bosses/XmasSnowman/xmas-snowman";

            InitialPosition = new Vector2(GameConfig.VirtualResolution.X / 2f, GameConfig.VirtualResolution.Y * 0.25f);
        }

        public override void Reset()
        {
            base.Reset();

            RandomMovingArea = new Rectangle(200, 350, GameConfig.VirtualResolution.X - 200, 650);
        }

        protected override void LoadSpriterSprite()
        {
            base.LoadSpriterSprite();

            SnowballAnimator = Animators.First(a => a.Entity != null && a.Entity.Name == "Snowball");
            BigArmsAnimator = Animators.First(a => a.Entity != null && a.Entity.Name == "BigArms");
            HatAnimator = Animators.First(a => a.Entity != null && a.Entity.Name == "Hat");
        }

        protected override void InitializePhysics(bool setupPhysicsWorld = false)
        {
            base.InitializePhysics(true);

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(0, 30), 0.85f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(0, -60), 0.65f));
        }

        protected override void UpdateBehaviourIndex()
        {
            base.UpdateBehaviourIndex();
        }
    }
}