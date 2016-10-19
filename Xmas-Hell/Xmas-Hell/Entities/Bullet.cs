using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace Xmas_Hell.Entities
{
    public class Bullet
    {
        protected XmasHell _game;
        public Sprite Sprite;
        public float Speed;

        public Bullet(XmasHell game, Vector2 position, float speed)
        {
            _game = game;
            Sprite = new Sprite(Assets.GetTexture2D("Graphics/Sprites/bullet"));
            Sprite.Position = position;
            Speed = speed;
        }

        public virtual void Update(GameTime gameTime)
        {
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            var angle = Sprite.Rotation;
            var direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            Sprite.Position += (new Vector2(Speed) * direction) * dt;
        }

        public virtual void Draw(GameTime gameTime)
        {
            Sprite.Draw(_game.SpriteBatch);
        }
    }
}