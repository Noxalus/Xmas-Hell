using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XmasHell.Background
{
    public class GradientBackground : AbstractBackground
    {
        private Vector2[] _metaballs;

        public GradientBackground(XmasHell game) : base(game)
        {
            BackgroundEffect = Assets.GetShader("Graphics/Shaders/AnimatedGradient");

            BackgroundEffect.Parameters["uGradientPoint0Color"].SetValue(new Color(0.53f, 0.8f, 0.88f, 1f).ToVector4());
            BackgroundEffect.Parameters["uGradientPoint1Color"].SetValue(new Color(0.24f, 0.67f, 0.82f, 1f).ToVector4());
            BackgroundEffect.Parameters["uSpeed"].SetValue(0.1f);
            BackgroundEffect.Parameters["uAmplitude"].SetValue(1f);
            BackgroundEffect.Parameters["uResolution"].SetValue(GameConfig.VirtualResolution.ToVector2());

        }

        public override void Update(GameTime gameTime)
        {
            BackgroundEffect.Parameters["uTime"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
        }
    }
}
