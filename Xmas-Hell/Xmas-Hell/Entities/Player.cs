using System;
using Android.OS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended.Sprites;

namespace Xmas_Hell.Entities
{
    class Player
    {
        private readonly XmasHell _game;
        private Sprite _sprite;
        private float _speed;

        private Vector2 _initialSpritePosition;
        private Vector2 _initialTouchPosition;
        private Vector2 _currentTouchPosition;

        public Player(XmasHell game)
        {
            _game = game;
            _speed = Config.PlayerSpeed;
        }

        public void LoadContent()
        {
            var playerTexture = _game.Content.Load<Texture2D>("Graphics/Sprites/player");

            _sprite = new Sprite(playerTexture)
            {
                Origin = new Vector2(playerTexture.Width / 2f, playerTexture.Height / 2f),
                Position = new Vector2(720f / 2f + (playerTexture.Width / 2f), 1280f - 150),
                Scale = Vector2.One
            };
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var currentTouchState = TouchPanel.GetState();

            if (currentTouchState.Count > 0)
            {
                if (_initialSpritePosition.Equals(Vector2.Zero))
                {
                    _initialSpritePosition = _sprite.Position;
                    _initialTouchPosition = currentTouchState[0].Position;
                }

                _currentTouchPosition = currentTouchState[0].Position;

                var touchDelta = _currentTouchPosition - _initialTouchPosition;

                _sprite.Position = _initialSpritePosition + (touchDelta * _speed) * dt;
            }
            else
            {
                _initialSpritePosition = Vector2.Zero;
                _initialTouchPosition = Vector2.Zero;
            }
        }

        public void Draw()
        {
            _game.SpriteBatch.Draw(_sprite);
        }
    }
}