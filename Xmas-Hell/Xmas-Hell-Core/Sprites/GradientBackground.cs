using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        }

        public void Update(GameTime gameTime)
        {
            _animatedGradientEffect.Parameters["gradientPoint0Color"].SetValue(Color.Green.ToVector3());
            _animatedGradientEffect.Parameters["gradientPoint1Color"].SetValue(Color.Red.ToVector3());

            _animatedGradientEffect.Parameters["gradientPoint0Position"].SetValue(Vector2.Zero);
            _animatedGradientEffect.Parameters["gradientPoint1Position"].SetValue(Vector2.One);
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
