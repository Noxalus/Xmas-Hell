using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace XmasHell.Background
{
    public class SnowRainBackground : AbstractBackground
    {
        private List<BackgroundSnowflake> _backgroundSnowflakes;
        private Effect _snowflakeEffect;

        public SnowRainBackground(XmasHell game) : base(game)
        {
            BackgroundEffect = Assets.GetShader("Graphics/Shaders/SnowRainBackground");
            _snowflakeEffect = Assets.GetShader("Graphics/Shaders/Snowflake");

            _backgroundSnowflakes = new List<BackgroundSnowflake>
            {
                new BackgroundSnowflake(game)
            };
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var backgroundSnowflake in _backgroundSnowflakes)
                backgroundSnowflake.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();

            Game.SpriteBatch.Begin(
                effect: _snowflakeEffect,
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transformMatrix: Game.ViewportAdapter.GetScaleMatrix()
            );

            foreach (var backgroundSnowflake in _backgroundSnowflakes)
                backgroundSnowflake.Draw();

            Game.SpriteBatch.End();
        }
    }
}