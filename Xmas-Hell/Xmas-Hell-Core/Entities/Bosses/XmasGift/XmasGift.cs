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
            Behaviours.Add(new XmasGiftBehaviour2(this));
        }

        protected override void InitializePhysics(bool setupPhysicsWorld = false)
        {
            base.InitializePhysics(true);

            // Physics
            var width = GetSpritePartWidth("body.png") * 0.9f;
            var height = GetSpritePartHeight("body.png") * 0.95f;
            var scale = 0.85f;
            var relativePosition = new Vector2(
                (1f - scale) / 2f * width,
                height - (height * scale)
            );

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(this, "body.png", relativePosition, scale));
        }

        protected override void SetupPhysicsWorld()
        {
            base.SetupPhysicsWorld();

            var width = GetSpritePartWidth("body.png") * 0.9f;
            var height = GetSpritePartHeight("body.png") * 0.95f;
            var position = ConvertUnits.ToSimUnits(Position());

            PhysicsBody = BodyFactory.CreateRectangle(
                PhysicsWorld,
                ConvertUnits.ToSimUnits(width),
                ConvertUnits.ToSimUnits(height),
                1f,
                position
            );

            PhysicsBody.BodyType = BodyType.Dynamic;
            PhysicsBody.Restitution = 0.01f;
            PhysicsBody.Friction = 0.5f;
            PhysicsBody.Mass = 30f;
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