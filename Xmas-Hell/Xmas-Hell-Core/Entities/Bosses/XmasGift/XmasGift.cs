using System;
using System.Collections.Generic;
using BulletML;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XmasHell.Geometry;
using XmasHell.Physics.Collision;
using Xmas_Hell_Core.Controls;
using Xmas_Hell_Core.Physics.DebugView;

namespace XmasHell.Entities.Bosses.XmasGift
{
    class XmasGift : Boss
    {
        // Physics World
        protected World World;
        protected Body GiftBody;
        protected Body LeftWallBody;
        protected Body TopWallBody;
        protected Body RightWallBody;
        protected Body BottomWallBody;

        private DebugView _debugView;

        public XmasGift(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasGift, playerPositionDelegate)
        {
            // Spriter
            SpriterFilename = "Graphics/Sprites/Bosses/XmasGift/xmas-gift";

            // BulletML
            BulletPatternFiles.Add("sample");

            // Behaviours
            Behaviours.Add(new XmasGiftBehaviour1(this, World));
        }

        protected override void InitializePhysics()
        {
            base.InitializePhysics();

            // Physics
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(this, "body.png"));
            var gravity = Vector2.UnitY * 9.82f;
            //gravity = Vector2.Zero;

            World = new World(gravity);

            if (GameConfig.DebugPhysics)
                _debugView = new DebugView(World, Game, 1f);

            SetupPhysicsWorld();
        }

        private void SetupPhysicsWorld()
        {
            var width = GetSpritePartWidth("body.png");
            var height = GetSpritePartHeight("body.png");
            var position = new Vector2(ConvertUnits.ToSimUnits(Position().X), ConvertUnits.ToSimUnits(Position().Y));

            GiftBody = BodyFactory.CreateRectangle(
                World,
                ConvertUnits.ToSimUnits(width),
                ConvertUnits.ToSimUnits(height),
                10f,
                position
            );

            GiftBody.BodyType = BodyType.Dynamic;
            GiftBody.Restitution = 1.01f;
            GiftBody.Friction = 0.1f;

            // Walls
            BottomWallBody = BodyFactory.CreateEdge(
                World,
                ConvertUnits.ToSimUnits(0, GameConfig.VirtualResolution.Y),
                ConvertUnits.ToSimUnits(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y)
            );

            LeftWallBody = BodyFactory.CreateEdge(
                World,
                ConvertUnits.ToSimUnits(0, 0),
                ConvertUnits.ToSimUnits(0, GameConfig.VirtualResolution.Y)
            );

            RightWallBody = BodyFactory.CreateEdge(
                World,
                ConvertUnits.ToSimUnits(GameConfig.VirtualResolution.X, 0),
                ConvertUnits.ToSimUnits(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y)
            );

            TopWallBody = BodyFactory.CreateEdge(
                World,
                ConvertUnits.ToSimUnits(0, 0),
                ConvertUnits.ToSimUnits(GameConfig.VirtualResolution.X, 0)
            );
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //if (InputManager.KeyDown(Keys.Space))
            {
                //var forceVector = new Vector2(0.5f, 0.5f);
                //var strength = 10f;

                //forceVector.Normalize();
                //_giftBody.ApplyForce(forceVector * strength);
                //_giftBody.ApplyLinearImpulse(new Vector2(100, 10));
                GiftBody.ApplyAngularImpulse(0.5f);

            }

            World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            SynchronizeGraphicsWithPhysics();
        }

        private void SynchronizeGraphicsWithPhysics()
        {
            Position(ConvertUnits.ToDisplayUnits(GiftBody.Position));
            Rotation(GiftBody.Rotation);
        }

        public override void Draw()
        {
            base.Draw();

            if (GameConfig.DebugPhysics)
            {
                var view = Game.Camera.GetViewMatrix();
                var projection = Matrix.CreateOrthographicOffCenter(
                    0f,
                    ConvertUnits.ToSimUnits(Game.GraphicsDevice.Viewport.Width),
                    ConvertUnits.ToSimUnits(Game.GraphicsDevice.Viewport.Height), 0f, 0f, 1f
                );

                _debugView.Draw(ref projection, ref view);
            }
        }
    }
}