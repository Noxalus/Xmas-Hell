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
            BackgroundEffect = Assets.GetShader("Graphics/Shaders/Theter");
            _metaballs = new Vector2[4];
        }

        public override void Update(GameTime gameTime)
        {
            // time / speed
            var time = (float) gameTime.TotalGameTime.TotalSeconds / 2f;
            var c = (float)Math.Cos(time);
            var s = (float)Math.Sin(time);

            _metaballs[0].X = c * 0.4f + 0.6f;
            _metaballs[0].Y = s * 0.3f + 0.4f;

            _metaballs[1].X = c * 0.5f + 0.70f;
            _metaballs[1].Y = (float)Math.Sin(time * 0.25) * 0.2f + 0.4f;

            _metaballs[2].X = (float)Math.Cos(time * 0.33) * 0.6f + 0.7f;
            _metaballs[2].Y = (float)Math.Sin(time * 1.5) * 0.3f + 0.4f;

            _metaballs[3].X = s * 0.4f + 0.6f;
            _metaballs[3].Y = c * 0.3f + 0.4f;

            BackgroundEffect.Parameters["uMetaBalls"].SetValue(_metaballs);
        }
    }
}
