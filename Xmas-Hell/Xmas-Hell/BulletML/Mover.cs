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
        private Sprite _sprite;

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
            //Texture = BulletTypeUtils.BulletTypeToTexture(BulletType.Type1);
        }

        public void Init(bool topBullet)
        {
            Used = true;

            _sprite = new Sprite(Texture);

            if (!topBullet)
            {
                _game.GameManager.CollisionWorld.BossBulletHitboxes.Add(
                    new CollisionCircle(this, Vector2.Zero, Texture.Width / 2f)
                );
            }
        }

        public override void Update()
        {
            base.Update();

            _sprite.Position = _position;
            _sprite.Rotation = Direction;
            _sprite.Scale = Scale();

            if (X < -100 || X > GameConfig.VirtualResolution.X + 100 || Y < -100 || Y > GameConfig.VirtualResolution.Y + 100)
                Used = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch);
        }
    }
}
