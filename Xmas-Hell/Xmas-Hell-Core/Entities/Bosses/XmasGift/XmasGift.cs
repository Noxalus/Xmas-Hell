using BulletML;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasGift
{
    class XmasGift : Boss
    {
        public XmasGift(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasGift, playerPositionDelegate)
        {
            // Spriter
            SpriterFilename = "Graphics/Sprites/Bosses/XmasGift/xmas-gift";

            // BulletML
            BulletPatternFiles.Add("XmasGift/pattern1");

            // Behaviours
            Behaviours.Add(new XmasGiftBehaviour1(this));
        }

        protected override void InitializePhysics(bool setupPhysicsWorld = false)
        {
            base.InitializePhysics(true);

            // Physics
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(this, "body.png", Vector2.Zero, 1f));
        }

        protected override void SetupPhysicsWorld()
        {
            base.SetupPhysicsWorld();

            var width = GetSpritePartWidth("body.png");
            var height = GetSpritePartHeight("body.png");
            var position = new Vector2(ConvertUnits.ToSimUnits(Position().X), ConvertUnits.ToSimUnits(Position().Y));

            PhysicsBody = BodyFactory.CreateRectangle(
                PhysicsWorld,
                ConvertUnits.ToSimUnits(width),
                ConvertUnits.ToSimUnits(height),
                1f,
                position
            );

            PhysicsBody.BodyType = BodyType.Dynamic;
            PhysicsBody.Restitution = 1.01f;
            PhysicsBody.Friction = 0.1f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}