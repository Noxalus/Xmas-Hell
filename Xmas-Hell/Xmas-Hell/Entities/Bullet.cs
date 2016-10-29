using System;
using Android.Provider;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using Xmas_Hell.Physics;
using Xmas_Hell.Physics.Collision;

namespace Xmas_Hell.Entities
{
    public class Bullet : IPhysicsEntity
    {
        protected XmasHell _game;
        public Sprite Sprite;
        public float Speed;
        public bool Used;

        public Vector2 Position()
        {
            return Sprite.Position;
        }

        public float Rotation()
        {
            return Sprite.Rotation;
        }

        public Vector2 Scale()
        {
            return Sprite.Scale;
        }

        public Bullet(XmasHell game, Vector2 position, float rotation, float speed)
        {
            _game = game;
            var defaultBulletTexture = Assets.GetTexture2D("Graphics/Sprites/bullet2");
            Sprite = new Sprite(defaultBulletTexture);
            Sprite.Position = position;
            Sprite.Rotation = rotation;
            Speed = speed;
            Used = true;

            _game.GameManager.CollisionWorld.PlayerBulletHitboxes.Add(
                new CollisionCircle(this, Vector2.Zero, defaultBulletTexture.Width / 2f)
            );
        }

        public void Destroy()
        {
            // TODO: Launch an animation
            Used = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            var angle = Sprite.Rotation - MathHelper.PiOver2;
            var direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            Sprite.Position += (new Vector2(Speed) * direction) * dt;

            CheckOutOfBounds();
        }

        private void CheckOutOfBounds()
        {
            if (Position().X < 0 || Position().X > GameConfig.VirtualResolution.X ||
                Position().Y < 0 || Position().Y > GameConfig.VirtualResolution.Y)
            {
                Used = false;
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            Sprite.Draw(_game.SpriteBatch);
        }
    }
}