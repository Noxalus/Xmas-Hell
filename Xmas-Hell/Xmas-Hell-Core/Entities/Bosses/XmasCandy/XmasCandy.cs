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