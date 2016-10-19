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

            UpdatePosition(dt);
            UpdateShoot(dt);
        }

        private void UpdatePosition(float dt)
        {
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

                //Console.WriteLine("Touch delta (before): " + touchDelta);

                //touchDelta.X /= Config.VirtualResolution.X;
                //touchDelta.Y /= Config.VirtualResolution.Y;

                //Console.WriteLine("Touch delta (after): " + touchDelta);

                _sprite.Position = _initialSpritePosition + (touchDelta * _speed) * dt;
            }
            else
            {
                _initialSpritePosition = Vector2.Zero;
                _initialTouchPosition = Vector2.Zero;
            }
        }

        private void UpdateShoot(float dt)
        {
            // TODO: Choose shoot frequency

            var bullet = new PlayerBullet(_game, _sprite.Position, 100f);

            _game.GameManager.AddPlayerBullet(bullet);
        }

        public void Draw()
        {
            _game.SpriteBatch.Draw(_sprite);
        }
    }
}