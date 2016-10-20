using System;
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

        private TimeSpan _bulletFrequence;

        public Player(XmasHell game)
        {
            _game = game;
            _speed = Config.PlayerSpeed;
            _bulletFrequence = new TimeSpan(0);

            var playerTexture = Assets.GetTexture2D("Graphics/Sprites/player");

            _sprite = new Sprite(playerTexture)
            {
                Origin = new Vector2(playerTexture.Width / 2f, playerTexture.Height / 2f),
                Position = new Vector2(
                    Config.VirtualResolution.X / 2f,
                    Config.VirtualResolution.Y - 150
                ),
                Scale = Vector2.One
            };
        }

        public void Update(GameTime gameTime)
        {
            UpdatePosition(gameTime);
            UpdateShoot(gameTime);
        }

        private void UpdatePosition(GameTime gameTime)
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

        private void UpdateShoot(GameTime gameTime)
        {
            if (_bulletFrequence.TotalMilliseconds > 0)
                _bulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                _bulletFrequence = TimeSpan.FromTicks(Config.PlayerShootFrequency.Ticks);

                var bulletSpeed = 750f;
                var bullet1 = new PlayerBullet(_game, _sprite.Position, -MathHelper.PiOver4 / 4f, bulletSpeed);
                var bullet2 = new PlayerBullet(_game, _sprite.Position, -MathHelper.PiOver4 / 8f, bulletSpeed);
                var bullet3 = new PlayerBullet(_game, _sprite.Position, 0f, bulletSpeed);
                var bullet4 = new PlayerBullet(_game, _sprite.Position, MathHelper.PiOver4 / 8f, bulletSpeed);
                var bullet5 = new PlayerBullet(_game, _sprite.Position, MathHelper.PiOver4 / 4f, bulletSpeed);

                _game.GameManager.AddBullet(bullet1);
                _game.GameManager.AddBullet(bullet2);
                _game.GameManager.AddBullet(bullet3);
                _game.GameManager.AddBullet(bullet4);
                _game.GameManager.AddBullet(bullet5);
            }
        }

        public void Draw(GameTime gameTime)
        {
            _game.SpriteBatch.Draw(_sprite);
        }
    }
}