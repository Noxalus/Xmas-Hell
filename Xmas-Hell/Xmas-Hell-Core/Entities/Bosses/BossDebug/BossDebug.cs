using BulletML;
using Microsoft.Xna.Framework;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.DebugBoss
{
    internal class BossDebug : Boss
    {
        public BossDebug(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.Debug, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("BossDebug/pattern1");

            // Behaviours
            Behaviours.Add(new BossDebugBehaviour1(this));

            SpriterFilename = "Graphics/Sprites/Bosses/BossDebug/boss-debug";
        }

        protected override void LoadSpriterSprite()
        {
            base.LoadSpriterSprite();
        }

        protected override void InitializePhysics(bool setupPhysicsWorld = false)
        {
            base.InitializePhysics();

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(0f, 10f), 0.90f));
        }

        public override void Reset()
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