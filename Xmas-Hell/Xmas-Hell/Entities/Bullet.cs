using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;

namespace Xmas_Hell.Entities
{
    public class Bullet
    {
        protected XmasHell _game;
        public Sprite Sprite;
        public float Speed;

        public Vector2 Position => Sprite.Position;

        public Bullet(XmasHell game, Vector2 position, float rotation, float speed)
        {
            _game = game;
            Sprite = new Sprite(Assets.GetTexture2D("Graphics/Sprites/bullet2"));
            Sprite.Position = position;
            Sprite.Rotation = rotation;
            Speed = speed;
        }

        public virtual void Update(GameTime gameTime)
        {
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            var angle = Sprite.Rotation - MathHelper.PiOver2;
            var direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            Sprite.Position += (new Vector2(Speed) * direction) * dt;
        }

        public virtual void Draw(GameTime gameTime)
        {
            Sprite.Draw(_game.SpriteBatch);
        }
    }
}