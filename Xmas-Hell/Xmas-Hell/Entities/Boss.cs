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
        private float _life;

        public Boss(XmasHell game, Vector2 position, float life)
        {
            _game = game;
            _sprite = new Sprite(Assets.GetTexture2D("Graphics/Sprites/boss"))
            {
                Position = position
            };

            _life = _life;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime)
        {
            _sprite.Draw(_game.SpriteBatch);
        }
    }
}