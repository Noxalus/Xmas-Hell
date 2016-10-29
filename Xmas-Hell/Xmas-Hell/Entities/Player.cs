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

        private Vector2 _initialSpritePosition;
        private Point _initialTouchPosition;

        private TimeSpan _bulletFrequence;

        public Vector2 Position()
        {
            return _sprite.Position;
        }

        public Player(XmasHell game)
        {
            _game = game;
            _bulletFrequence = TimeSpan.Zero;

            var playerTexture = Assets.GetTexture2D("Graphics/Sprites/player");

            _sprite = new Sprite(playerTexture)
            {
                Origin = new Vector2(playerTexture.Width / 2f, playerTexture.Height / 2f),
                Position = new Vector2(
                    GameConfig.VirtualResolution.X / 2f,
                    GameConfig.VirtualResolution.Y - 150
                ),
                Scale = Vector2.One
            };

            // Don't forget to set the player position delegate to the MoverManager
            _game.GameManager.MoverManager.SetPlayerPositionDelegate(Position);
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
                if (currentTouchState[0].State == TouchLocationState.Pressed)
                {
                    _initialSpritePosition = _sprite.Position;
                    _initialTouchPosition = _game.ViewportAdapter.PointToScreen(currentTouchState[0].Position.ToPoint());
                }

                var currentTouchPosition = _game.ViewportAdapter.PointToScreen(currentTouchState[0].Position.ToPoint());
                var touchDelta = (currentTouchPosition - _initialTouchPosition).ToVector2();

                _sprite.Position = _initialSpritePosition + (touchDelta * GameConfig.PlayerMoveSensitivity);
            }
            else
            {
                _initialSpritePosition = Vector2.Zero;
                _initialTouchPosition = Point.Zero;
            }
        }

        private void UpdateShoot(GameTime gameTime)
        {
            if (_bulletFrequence.TotalMilliseconds > 0)
                _bulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                _bulletFrequence = TimeSpan.FromTicks(GameConfig.PlayerShootFrequency.Ticks);

                var bullet1 = new PlayerBullet(_game, _sprite.Position, -MathHelper.PiOver4 / 4f, GameConfig.PlayerBulletSpeed);
                var bullet2 = new PlayerBullet(_game, _sprite.Position, -MathHelper.PiOver4 / 8f, GameConfig.PlayerBulletSpeed);
                var bullet3 = new PlayerBullet(_game, _sprite.Position, 0f, GameConfig.PlayerBulletSpeed);
                var bullet4 = new PlayerBullet(_game, _sprite.Position, MathHelper.PiOver4 / 8f, GameConfig.PlayerBulletSpeed);
                var bullet5 = new PlayerBullet(_game, _sprite.Position, MathHelper.PiOver4 / 4f, GameConfig.PlayerBulletSpeed);

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