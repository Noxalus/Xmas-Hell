using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using XmasHell.BulletML;
using XmasHell.Physics;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities
{
    public class Bullet : IPhysicsEntity
    {
        protected XmasHell _game;
        public Sprite Sprite;
        public float Speed;
        public bool Used;

        protected CollisionElement Hitbox;

        public Vector2 Position()
        {
            return Sprite.Position;
        }

        public virtual Vector2 LocalPosition()
        {
            return Vector2.Zero;
        }

        public float Rotation()
        {
            return Sprite.Rotation;
        }

        public Vector2 Origin()
        {
            return Sprite.Origin;
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

            Hitbox = new CollisionCircle(this, Vector2.Zero, defaultBulletTexture.Width/2f);
            _game.GameManager.CollisionWorld.AddPlayerBulletHitbox(Hitbox);

            _game.SpriteBatchManager.GameSprites.Add(Sprite);
        }

        public void Destroy()
        {
            // TODO: Launch an animation (or particles)
            Used = false;
            _game.SpriteBatchManager.GameSprites.Remove(Sprite);
            _game.GameManager.CollisionWorld.RemovePlayerBulletHitbox(Hitbox);
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
            if (Position().X < 100 || Position().X > GameConfig.VirtualResolution.X - 100 ||
                Position().Y < 100 || Position().Y > GameConfig.VirtualResolution.Y -100)
            {
                Destroy();
            }
        }

        public void TakeDamage(float damage)
        {
            // Nothing
        }
    }
}