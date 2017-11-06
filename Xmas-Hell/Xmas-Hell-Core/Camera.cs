using System;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Timers;
using MonoGame.Extended.ViewportAdapters;
using XmasHell.Sound;

namespace XmasHell
{
    public class Camera : Camera2D
    {
        private XmasHell _game;
        private bool _shaking;
        private float _shakingMagnitude;
        private TimeSpan _shakingTimer;
        private Vector2 _initialPosition;

        private float _initialZoom;
        private bool _targetingZoom;
        private float _targetZoom;
        private CountdownTimer _targetZoomTimer;

        private PositionDelegate _followPositionDelegate;
        private bool _followingPosition;

        public Camera(XmasHell game, GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            _game = game;
            _targetingZoom = false;
            _targetZoom = 1f;

            _followPositionDelegate = null;
            _followingPosition = false;
        }

        public Camera(XmasHell game, ViewportAdapter viewportAdapter) : base(viewportAdapter)
        {
            _game = game;
            _targetingZoom = false;
            _targetZoom = 1f;

            _followPositionDelegate = null;
            _followingPosition = false;

            MaximumZoom = 5f;
            MinimumZoom = 0.01f;
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

            if (_followingPosition && _followPositionDelegate != null)
            {
                Origin = _followPositionDelegate();
            }

            if (_targetingZoom)
            {
                _targetZoomTimer.Update(gameTime);

                var delta = _targetZoomTimer.CurrentTime.TotalSeconds / _targetZoomTimer.Interval.TotalSeconds;
                var currentZoom = MathHelper.Lerp(_initialZoom, _targetZoom, (float)delta);
                Zoom = currentZoom;

                if (Math.Abs(Zoom - _targetZoom) < 0.0001f)
                {
                    Zoom = _targetZoom;
                    _targetingZoom = false;
                }
            }
        }

        // Time in seconds
        public void Shake(float time, float magnitude)
        {
            SoundManager.PlaySound(Assets.GetSound("Audio/SE/shake"));

            if (_shaking)
                return;

            _shaking = true;
            _shakingMagnitude = magnitude;
            _shakingTimer = TimeSpan.FromSeconds(time);
            _initialPosition = Position;
        }

        public void ZoomTo(float targetZoom, double time, Vector2? origin = null)
        {
            if (_targetingZoom)
                return;

            _initialZoom = Zoom;
            _targetZoom = targetZoom;
            _targetingZoom = true;
            _targetZoomTimer = new CountdownTimer(time);
            _targetZoomTimer.Start();

            if (origin.HasValue)
                Origin = origin.Value;
        }

        public void FollowPosition(PositionDelegate positionDelegate)
        {
            _followPositionDelegate = positionDelegate;
            _followingPosition = true;
        }
    }
}