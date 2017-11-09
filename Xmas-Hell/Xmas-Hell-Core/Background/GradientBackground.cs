using Microsoft.Xna.Framework;

namespace XmasHell.Background
{
    public class GradientBackground : AbstractBackground
    {
        private Vector2[] _metaballs;

        public GradientBackground(XmasHell game) : base(game)
        {
            BackgroundEffect = Assets.GetShader("Graphics/Shaders/AnimatedGradient");

            BackgroundEffect.Parameters["uGradientPoint0Color"].SetValue(Color.White.ToVector4());
            BackgroundEffect.Parameters["uGradientPoint1Color"].SetValue(Color.Black.ToVector4());
            BackgroundEffect.Parameters["uSpeed"].SetValue(0.5f);
            BackgroundEffect.Parameters["uInnerAmplitude"].SetValue(2.5f);
            BackgroundEffect.Parameters["uOuterAmplitude"].SetValue(1.5f);
            BackgroundEffect.Parameters["uResolution"].SetValue(GameConfig.VirtualResolution.ToVector2());
        }

        public void ChangeGradientColors(Color brightColor, Color darkColor)
        {
            BackgroundEffect.Parameters["uGradientPoint0Color"].SetValue(brightColor.ToVector4());
            BackgroundEffect.Parameters["uGradientPoint1Color"].SetValue(darkColor.ToVector4());
        }

        public override void Update(GameTime gameTime)
        {
            BackgroundEffect.Parameters["uTime"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
        }
    }
}