using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace Xmas_Hell
{
    public class Camera : Camera2D
    {
        private XmasHell _game;
        private bool _shaking;
        private float _shakingMagnitude;
        private TimeSpan _shakingTimer;
        private Vector2 _initialPosition;

        public Camera(XmasHell game, GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            _game = game;
        }

        public Camera(XmasHell game, ViewportAdapter viewportAdapter) : base(viewportAdapter)
        {
            _game = game;
        }

        public void Update(GameTime gameTime)
        {
            if (_shaking)
            {
                if (_shakingTimer.TotalMilliseconds > 0)
                {
                    _shakingTimer -= gameTime.ElapsedGameTime;
                    Position = _initialPosition + new Vector2((float)(_game.GameManager.Random.NextDouble() * _shakingMagnitude));
                }
                else
                {
                    _shaking = false;
                    _shakingTimer = TimeSpan.Zero;
                    Position = _initialPosition;
                }
            }
        }

        // Time in seconds
        public void Shake(float time, float magnitude)
        {
            if (_shaking)
                return;

            _shaking = true;
            _shakingMagnitude = magnitude;
            _shakingTimer = TimeSpan.FromSeconds(time);
            _initialPosition = Position;
        }
    }
}