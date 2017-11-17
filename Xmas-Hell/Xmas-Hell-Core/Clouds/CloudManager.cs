using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace XmasHell.Entities
{
    public class CloudManager
    {
        private XmasHell _game;
        private float _speed;
        private int _direction;
        private TimeSpan _cloudTimer;
        private Sprite _sprite;

        private List<Sprite> _cloudSprites;

        public CloudManager(XmasHell game)
        {
            _game = game;

            _cloudTimer = TimeSpan.FromSeconds(_game.GameManager.Random.NextDouble() * 20f);
        }

        public void LoadContent()
        {
            _cloudSprites = new List<Sprite>()
            {
                new Sprite(Assets.GetTexture2D("Graphics/GUI/cloud1")),
                new Sprite(Assets.GetTexture2D("Graphics/GUI/cloud2"))
            };
        }

        public void Dispose()
        {
            _sprite = null;
        }

        private void CreateRandomCloud()
        {
            var randomCloudIndex = _game.GameManager.Random.Next(_cloudSprites.Count);
            _sprite = _cloudSprites[randomCloudIndex];

            _direction = (_game.GameManager.Random.NextDouble() > 0.5) ? 1 : -1;

            _sprite.Position = new Vector2(
                _direction == 1 ? -_sprite.BoundingRectangle.Width : GameConfig.VirtualResolution.X + _sprite.BoundingRectangle.Width,
                _game.GameManager.Random.Next(0, 300)
            );

            _speed = _game.GameManager.Random.Next(50, 150);
        }

        public void Update(GameTime gameTime)
        {
            if (_cloudTimer.TotalMilliseconds < 0)
            {
                if (_sprite == null)
                    CreateRandomCloud();

                _sprite.Position = new Vector2(
                    _sprite.Position.X + _direction * _speed * gameTime.GetElapsedSeconds(),
                    _sprite.Position.Y
                );

                if (_direction == 1 && _sprite.Position.X > GameConfig.VirtualResolution.X + _sprite.BoundingRectangle.Width ||
                    _direction == -1 && _sprite.Position.X < -_sprite.BoundingRectangle.Width)
                {
                    _sprite = null;
                    _cloudTimer = TimeSpan.FromSeconds(_game.GameManager.Random.NextDouble() * 20f);
                }
            }
            else
                _cloudTimer -= gameTime.ElapsedGameTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _sprite?.Draw(spriteBatch);
        }
    }
}
