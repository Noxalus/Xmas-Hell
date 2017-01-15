using System.Collections.Generic;
using BulletML;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasGift
{
    class XmasGift : Boss
    {
        // Physics World
        private World _world;
        private Body _giftBody;
        private Body _leftWallBody;
        private Body _topWallBody;
        private Body _rightWallBody;
        private Body _bottomWallBody;

        public XmasGift(XmasHell game, PositionDelegate playerPositionDelegate) : base(game, playerPositionDelegate)
        {
            // Spriter
            SpriterFilename = "Graphics/Sprites/Bosses/XmasGift/xmas-gift";

            // BulletML
            BulletPatternFiles.Add("sample");

            // Physics
            Game.GameManager.CollisionWorld.BossHitbox = new SpriterCollisionCircle(this, "body.png", new Vector2(0f, 10f), 0.90f);
            var gravity = new Vector2(0f, 9.82f);
            _world = new World(gravity);

            // Behaviours
            Behaviours.Add(new XmasGiftBehaviour1(this, _world));
        }

        public override void Initialize()
        {
            base.Initialize();

            SetupPhysicsWorld();
        }

        private void SetupPhysicsWorld()
        {
            _giftBody = BodyFactory.CreateBody(_world);
            _giftBody.BodyType = BodyType.Dynamic;
            _giftBody.Position = Position();
            //_giftBody.LinearDamping = 0f;
            //_giftBody.AngularDamping = 0f;
            //_giftBody.LinearVelocity = new Vector2(0, 100);
            //_giftBody.Mass = 250f;

            var giftShape = new PolygonShape(new Vertices(new List<Vector2>
            {
                new Vector2(0, 0),
                new Vector2(Width(), 0),
                new Vector2(Width(), Height()),
                new Vector2(0, Height())
            }), 1f);

            // Walls
            _bottomWallBody = BodyFactory.CreateBody(_world);
            _leftWallBody = BodyFactory.CreateBody(_world);
            _rightWallBody = BodyFactory.CreateBody(_world);
            _topWallBody = BodyFactory.CreateBody(_world);

            var bottomWallShape = new EdgeShape(
                new Vector2(0, Game.Graphics.GraphicsDevice.Viewport.Height),
                new Vector2(Game.Graphics.GraphicsDevice.Viewport.Width, Game.Graphics.GraphicsDevice.Viewport.Height)
            );

            var leftWallShape = new EdgeShape(
                new Vector2(0, 0),
                new Vector2(0, Game.Graphics.GraphicsDevice.Viewport.Height)
            );

            var rightWallShape = new EdgeShape(
                new Vector2(Game.Graphics.GraphicsDevice.Viewport.Width, 0),
                new Vector2(Game.Graphics.GraphicsDevice.Viewport.Width, Game.Graphics.GraphicsDevice.Viewport.Height)
            );

            var topWallShape = new EdgeShape(
                new Vector2(0, 0),
                new Vector2(0, Game.Graphics.GraphicsDevice.Viewport.Height)
            );

            // Fixtures
            var fixture = _giftBody.CreateFixture(giftShape);
            var bottomWallFixture = _bottomWallBody.CreateFixture(bottomWallShape);
            var leftWallFixture = _leftWallBody.CreateFixture(leftWallShape);
            var rightWallFixture = _rightWallBody.CreateFixture(rightWallShape);
            var topWallFixture = _topWallBody.CreateFixture(topWallShape);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            // Update graphics from physics
            Position(_giftBody.Position);
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}