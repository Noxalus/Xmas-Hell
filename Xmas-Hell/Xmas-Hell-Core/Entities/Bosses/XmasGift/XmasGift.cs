using System;
using System.Collections.Generic;
using BulletML;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XmasHell.Physics.Collision;
using Xmas_Hell_Core.Controls;
using Xmas_Hell_Core.Physics.DebugView;

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

        private Fixture _giftFixture;

        private DebugView _debugView;
        public override float Width()
        {
            if (CurrentAnimator != null)
            {
                var spriteBodyPart = Array.Find(CurrentAnimator.Entity.Spriter.Folders[0].Files, (file) => file.Name == "body.png");
                return spriteBodyPart.Height;
            }

            return 0f;
        }

        public override float Height()
        {
            if (CurrentAnimator != null)
            {
                var spriteBodyPart = Array.Find(CurrentAnimator.Entity.Spriter.Folders[0].Files, (file) => file.Name == "body.png");
                return spriteBodyPart.Width;
            }

            return 0f;
        }

        public XmasGift(XmasHell game, PositionDelegate playerPositionDelegate) : base(game, playerPositionDelegate)
        {
            // Spriter
            SpriterFilename = "Graphics/Sprites/Bosses/XmasGift/xmas-gift";

            // BulletML
            BulletPatternFiles.Add("sample");

            // Physics
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(0f, 10f), 0.90f));
            var gravity = new Vector2(0f, 9.82f);

            _world = new World(gravity);

            _debugView = new DebugView(_world, Game, 1f);

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
            _giftBody.LinearDamping = -10f;
            //_giftBody.AngularDamping = 0f;
            //_giftBody.LinearVelocity = new Vector2(0, 100);
            //_giftBody.Mass = 250f;

            var width = Height();
            var height = Width();

            var giftShape = new PolygonShape(new Vertices(new List<Vector2>
            {
                new Vector2(-width / 2f, -height / 2f),
                new Vector2(width / 2f, -height / 2f),
                new Vector2(width / 2f, height / 2f),
                new Vector2(-width / 2f, height / 2f)
            }), 1f);

            // Walls
            _bottomWallBody = BodyFactory.CreateBody(_world);
            _leftWallBody = BodyFactory.CreateBody(_world);
            _rightWallBody = BodyFactory.CreateBody(_world);
            _topWallBody = BodyFactory.CreateBody(_world);

            var bottomWallShape = new EdgeShape(
                new Vector2(0, GameConfig.VirtualResolution.Y),
                new Vector2(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y)
            );

            var leftWallShape = new EdgeShape(
                new Vector2(0, 0),
                new Vector2(0, GameConfig.VirtualResolution.Y)
            );

            var rightWallShape = new EdgeShape(
                new Vector2(GameConfig.VirtualResolution.X, 0),
                new Vector2(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y)
            );

            var topWallShape = new EdgeShape(
                new Vector2(0, 0),
                new Vector2(0, GameConfig.VirtualResolution.Y)
            );

            // Fixtures
            _giftFixture = _giftBody.CreateFixture(giftShape);
            var bottomWallFixture = _bottomWallBody.CreateFixture(bottomWallShape);
            var leftWallFixture = _leftWallBody.CreateFixture(leftWallShape);
            var rightWallFixture = _rightWallBody.CreateFixture(rightWallShape);
            var topWallFixture = _topWallBody.CreateFixture(topWallShape);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.KeyDown(Keys.Space))
            {
                //_giftBody.ApplyLinearImpulse(new Vector2(1000, 10));
                //_giftBody.ApplyAngularImpulse(10f);
                var forceVector = new Vector2(0.5f, 0.5f);
                var strength = 10f;
                forceVector.Normalize();
                _giftBody.ApplyForce(forceVector * strength);
            }

            _world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            // Update graphics from physics
            Position(_giftBody.Position);
        }

        public override void Draw()
        {
            base.Draw();

            var projection = Matrix.CreateOrthographicOffCenter(0f, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
            var view = Game.Camera.GetViewMatrix();

            _debugView.Draw(ref projection, ref view);
        }
    }
}