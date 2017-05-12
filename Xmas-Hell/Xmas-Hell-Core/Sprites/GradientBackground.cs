using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RandomExtension;

namespace XmasHell.Sprites
{
    public class GradientBackground
    {
        private XmasHell _game;
        private Effect _animatedGradientEffect;

        public GradientBackground(XmasHell game)
        {
            _game = game;
            _animatedGradientEffect = Assets.GetShader("Graphics/Shaders/AnimatedGradient");

            _animatedGradientEffect.Parameters["gradientPoint0Color"].SetValue(Color.Black.ToVector3());
            _animatedGradientEffect.Parameters["gradientPoint1Color"].SetValue(new Color(0, 22, 83).ToVector3());
            _animatedGradientEffect.Parameters["gradientPoint0Position"].SetValue(new Vector2(0.5f, 0f));
            _animatedGradientEffect.Parameters["gradientPoint1Position"].SetValue(new Vector2(0.5f, 1f));
        }

        private Vector2 GetNextPosition()
        {
            return _game.GameManager.GetRandomPosition();
        }

        public void Update(GameTime gameTime)
        {
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
