using BulletML;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using XmasHell.Physics;
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
            Behaviours.Add(new XmasGiftBehaviour3(this));
        }

        protected override void InitializePhysics(bool setupPhysicsWorld = false)
        {
            base.InitializePhysics(true);

            Game.GameManager.CollisionWorld.AddBossHitBox(CreateBoundingBox(this));
        }

        public CollisionElement CreateBoundingBox(ISpriterPhysicsEntity spriterPhysicsEntity)
        {
            var width = GetSpritePartWidth("body.png") * 0.9f;
            var height = GetSpritePartHeight("body.png") * 0.95f;
            var scale = 0.85f;
            var relativePosition = new Vector2(
                (1f - scale) / 2f * width,
                height - (height * scale)
            );

            return new SpriterCollisionConvexPolygon(spriterPhysicsEntity, "body.png", relativePosition, scale);
        }

        public Body CreateGiftBody(Vector2 position)
        {
            var width = GetSpritePartWidth("body.png") * 0.9f;
            var height = GetSpritePartHeight("body.png") * 0.95f;
            var physicsPosition = ConvertUnits.ToSimUnits(position);

            var body = BodyFactory.CreateRectangle(
                PhysicsWorld,
                ConvertUnits.ToSimUnits(width),
                ConvertUnits.ToSimUnits(height),
                1f,
                physicsPosition
            );

            body.BodyType = BodyType.Dynamic;
            body.Restitution = 0.01f;
            body.Friction = 0.5f;
            body.Mass = 30f;

            return body;
        }

        protected override void SetupPhysicsWorld()
        {
            base.SetupPhysicsWorld();

            PhysicsBody = CreateGiftBody(Position());
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
        }

        protected override void UpdateBehaviourIndex()
        {
            base.UpdateBehaviourIndex();
        }
    }
}