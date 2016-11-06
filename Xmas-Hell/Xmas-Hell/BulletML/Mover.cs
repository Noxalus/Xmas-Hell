using System.Collections.Generic;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;
using Xmas_Hell.Physics;
using Xmas_Hell.Physics.Collision;

namespace Xmas_Hell.BulletML
{
    public class Mover : Bullet, IPhysicsEntity
    {
        private XmasHell _game;

        private Vector2 _position;
        public Texture2D Texture;
        public Sprite Sprite;

        public Vector2 Position()
        {
            return _position;
        }

        public float Rotation()
        {
            return Direction;
        }

        public Vector2 Scale()
        {
            // TODO: Get the proper scale
            return Vector2.One;
        }

        public void Position(Vector2 value)
        {
            _position = value;
        }

        public override float X
        {
            get { return Position().X; }
            set { _position.X = value; }
        }

        public override float Y
        {
            get { return Position().Y; }
            set { _position.Y = value; }
        }

        public bool Used { get; set; }

        public Mover(XmasHell game, IBulletManager bulletManager) : base(bulletManager)
        {
            _game = game;
        }

        public void Init(bool topBullet)
        {
            Used = true;

            Sprite = new Sprite(Texture);
            _game.SpriteBatchManager.BossBullets.Add(this);

            if (!topBullet)
            {
                _game.GameManager.CollisionWorld.BossBulletHitboxes.Add(
                    new CollisionCircle(this, Vector2.Zero, Texture.Width / 2f)
                );
            }
        }

        public void Destroy()
        {
            Used = false;
            _game.SpriteBatchManager.BossBullets.Remove(this);

            // TODO: Animation or particles
        }

        public override void Update()
        {
            base.Update();

            Sprite.Position = _position;
            Sprite.Rotation = Direction;
            Sprite.Scale = Scale();

            if (X < -100 || X > GameConfig.VirtualResolution.X + 100 || Y < -100 ||
                Y > GameConfig.VirtualResolution.Y + 100)
            {
                Destroy();
            }
        }
    }
}
