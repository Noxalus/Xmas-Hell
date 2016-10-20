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
using MonoGame.Extended.Sprites;

namespace Xmas_Hell.Entities
{
    class Boss
    {
        private XmasHell _game;
        private Sprite _sprite;
        private float _initialLife;
        private float _life;

        public Boss(XmasHell game, Vector2 position, float initialLife)
        {
            _game = game;
            _sprite = new Sprite(Assets.GetTexture2D("Graphics/Sprites/boss"))
            {
                Position = position
            };

            _initialLife = initialLife;
            _life = initialLife;
        }

        public void Update(GameTime gameTime)
        {
            _life -= 0.01f;
        }

        public void Draw(GameTime gameTime)
        {
            _sprite.Draw(_game.SpriteBatch);

            var percent = _life / _initialLife;
            _game.SpriteBatch.Draw(
                Assets.GetTexture2D("Graphics/Pictures/pixel"),
                new Rectangle(0, 0, (int)(percent * Config.VirtualResolution.X), 20),
                Color.Black
            );
        }
    }
}