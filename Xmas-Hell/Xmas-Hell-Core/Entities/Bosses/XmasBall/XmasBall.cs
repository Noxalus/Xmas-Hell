using BulletML;
using Microsoft.Xna.Framework;
using XmasHell.Physics;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasBall
{
    class XmasBall : Boss
    {
        public XmasBall(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasBall, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("XmasBall/pattern1");
            BulletPatternFiles.Add("XmasBall/pattern2");
            BulletPatternFiles.Add("XmasBall/pattern3");
            BulletPatternFiles.Add("XmasBall/pattern4");

            // Behaviours
            Behaviours.Add(new XmasBallBehaviour1(this));
            Behaviours.Add(new XmasBallBehaviour2(this));
            Behaviours.Add(new XmasBallBehaviour3(this));
            Behaviours.Add(new XmasBallBehaviour4(this));

            SpriterFilename = "Graphics/Sprites/Bosses/XmasBall/xmas-ball";
        }

        protected override void LoadSpriterSprite()
        {
            base.LoadSpriterSprite();
        }

        protected override void InitializePhysics()
        {
            base.InitializePhysics();

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(0f, 10f), 0.90f));
        }

        protected override void Reset()
        {
            base.Reset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void UpdateBehaviourIndex()
        {
            base.UpdateBehaviourIndex();
        }
    }
}