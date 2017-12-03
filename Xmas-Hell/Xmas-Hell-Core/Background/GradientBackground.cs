using Microsoft.Xna.Framework;

namespace XmasHell.Background
{
    public class GradientBackground : AbstractBackground
    {
        private Color _brightColor = Color.White;
        private Color _darkColor = Color.Black;

        public GradientBackground(XmasHell game) : base(game)
        {
            BackgroundEffect = Assets.GetShader("Graphics/Shaders/AnimatedGradient");

            BackgroundEffect.Parameters["uGradientPoint0Color"].SetValue(_brightColor.ToVector4());
            BackgroundEffect.Parameters["uGradientPoint1Color"].SetValue(_darkColor.ToVector4());
            BackgroundEffect.Parameters["uSpeed"].SetValue(0.5f);
            BackgroundEffect.Parameters["uInnerAmplitude"].SetValue(2.5f);
            BackgroundEffect.Parameters["uOuterAmplitude"].SetValue(1.5f);
            BackgroundEffect.Parameters["uResolution"].SetValue(GameConfig.VirtualResolution.ToVector2());
        }

        public Color BrightColor()
        {
            return _brightColor;
        }

        public Color DarkColor()
        {
            return _darkColor;
        }

        public void ChangeGradientColors(Color brightColor, Color darkColor)
        {
            _brightColor = brightColor;
            _darkColor = darkColor;

            BackgroundEffect.Parameters["uGradientPoint0Color"].SetValue(_brightColor.ToVector4());
            BackgroundEffect.Parameters["uGradientPoint1Color"].SetValue(_darkColor.ToVector4());
        }

        public override void Update(GameTime gameTime)
        {
            BackgroundEffect.Parameters["uTime"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
        }
    }
}