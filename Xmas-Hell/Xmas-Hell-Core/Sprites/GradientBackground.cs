using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XmasHell.Sprites
{
    public class GradientBackground
    {
        private XmasHell _game;
        private Effect _animatedGradientEffect;

        private Vector2 _gradientPoint0Position;
        private Color _gradientPoint0Color;
        private TimeSpan _newPositionTimer;
        private bool _targetingPosition;
        private Vector2 _targetPosition;
        private Vector2 _initialPosition;
        private TimeSpan _targetPositionTime;
        private TimeSpan _targetPositionTimer;

        public GradientBackground(XmasHell game)
        {
            _game = game;
            _animatedGradientEffect = Assets.GetShader("Graphics/Shaders/AnimatedGradient");

            _newPositionTimer = TimeSpan.Zero;
            _targetingPosition = false;
            _targetPosition = Vector2.Zero;
            _initialPosition = Vector2.Zero;
            _targetPositionTime = TimeSpan.FromSeconds(5);
            _targetPositionTimer = TimeSpan.FromSeconds(5);

            _gradientPoint0Color = Color.Black;
            _gradientPoint0Position = new Vector2(0.5f, 1f);

            _animatedGradientEffect.Parameters["gradientPoint0Color"].SetValue(_gradientPoint0Color.ToVector3());
            _animatedGradientEffect.Parameters["gradientPoint1Color"].SetValue(new Color(0, 22, 83).ToVector3());
            _animatedGradientEffect.Parameters["gradientPoint0Position"].SetValue(new Vector2(0.5f, 0f));
            _animatedGradientEffect.Parameters["gradientPoint1Position"].SetValue(new Vector2(0.5f, 1f));
        }

        public void Update(GameTime gameTime)
        {
            if (_newPositionTimer.TotalMilliseconds > 0)
            {
                _newPositionTimer -= gameTime.ElapsedGameTime;
            }
            else
            {
                if (!_targetingPosition)
                {
                    _newPositionTimer = TimeSpan.FromSeconds(0);
                    _targetPosition = _game.GameManager.GetRandomPosition(true);
                    _initialPosition = _gradientPoint0Position;
                    _targetPositionTime = TimeSpan.FromSeconds(5);
                    _targetingPosition = true;
                }
            }

            if (_targetingPosition)
            {
                var newPosition = Vector2.Zero;
                var lerpAmount = (float)(_targetPositionTime.TotalSeconds / _targetPositionTimer.TotalSeconds);

                newPosition.X = MathHelper.SmoothStep(_targetPosition.X, _initialPosition.X, lerpAmount);
                newPosition.Y = MathHelper.SmoothStep(_targetPosition.Y, _initialPosition.Y, lerpAmount);

                if (lerpAmount < 0.001f)
                {
                    _targetingPosition = false;
                    _targetPositionTime = TimeSpan.Zero;
                    _gradientPoint0Position = _targetPosition;
                }
                else
                    _targetPositionTime -= gameTime.ElapsedGameTime;

                _gradientPoint0Position = newPosition;
            }

            _animatedGradientEffect.Parameters["gradientPoint0Position"].SetValue(_gradientPoint0Position);

            //_animatedGradientEffect.Parameters["gradientPoint0Color"].SetValue(_game.GameManager.Random.NextColor().ToVector3());
            //_animatedGradientEffect.Parameters["gradientPoint1Color"].SetValue(_game.GameManager.Random.NextColor().ToVector3());
            //_animatedGradientEffect.Parameters["gradientPoint0Position"].SetValue(_game.GameManager.GetRandomPosition());
            //_animatedGradientEffect.Parameters["gradientPoint1Position"].SetValue(_game.GameManager.GetRandomPosition());
        }

        public void Draw()
        {
            _game.SpriteBatch.Begin(
                effect: _animatedGradientEffect,
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transformMatrix: _game.ViewportAdapter.GetScaleMatrix()
            );

            _game.SpriteBatch.Draw(Assets.GetTexture2D("pixel"), new Rectangle(0, 0, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y), null, Color.White);

            _game.SpriteBatch.End();
        }
    }
}
