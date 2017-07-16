using BulletML;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandy : Boss
    {
        public XmasCandy(XmasHell game, PositionDelegate playerPositionDelegate) : base(game, playerPositionDelegate)
        {
            InitialLife = 500f;

            // BulletML

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

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png"));

            // TODO: Create 3 rectangle bounding boxes for the body.png part
            // and one rectangle bounding box for each body2.png and body3.png parts
        }

        protected override void UpdateBehaviourIndex()
        {
            base.UpdateBehaviourIndex();
        }
    }
}