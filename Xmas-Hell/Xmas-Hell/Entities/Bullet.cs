using System;
using Android.Provider;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using Xmas_Hell.BulletML;
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
            var defaultBulletTexture = BulletTypeUtils.BulletTypeToTexture(BulletType.Type2);
            Sprite = new Sprite(defaultBulletTexture)
            {
                Position = position,
                Rotation = rotation
            };
            Speed = speed;
            Used = true;

            _game.GameManager.CollisionWorld.PlayerBulletHitboxes.Add(
                new CollisionCircle(this, Vector2.Zero, defaultBulletTexture.Width / 2f)
            );

            _game.SpriteBatchManager.GameSprites.Add(Sprite);
        }

        public void Destroy()
        {
            // TODO: Launch an animation (or particles)
            Used = false;
            _game.SpriteBatchManager.GameSprites.Remove(Sprite);
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
                Destroy();
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            Sprite.Draw(_game.SpriteBatch);
        }
    }
}