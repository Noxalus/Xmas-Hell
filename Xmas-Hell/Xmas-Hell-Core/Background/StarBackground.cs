using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XmasHell.Background
{
    public class StarBackground : AbstractBackground
    {
        private Effect _starBackgroundEffect;

        public StarBackground(XmasHell game) : base(game)
        {
            _starBackgroundEffect = Assets.GetShader("Graphics/Shaders/StarBackground");
        }

        public override void Update(GameTime gameTime)
        {
            // Change speed/orientation
        }

        public override void Draw()
        {
            Game.SpriteBatch.Begin(
                effect: _starBackgroundEffect,
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transformMatrix: Game.ViewportAdapter.GetScaleMatrix()
            );

            Game.SpriteBatch.Draw(Assets.GetTexture2D("pixel"), new Rectangle(0, 0, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y), null, Color.White);

            Game.SpriteBatch.End();
        }
    }
}
