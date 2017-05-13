using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XmasHell.Background
{
    public abstract class AbstractBackground
    {
        protected XmasHell Game;
        protected Effect BackgroundEffect;

        protected AbstractBackground(XmasHell game)
        {
            Game = game;
        }

        public abstract void Update(GameTime gameTime);

        public virtual void Draw()
        {
            Game.SpriteBatch.Begin(
                effect: BackgroundEffect,
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transformMatrix: Game.ViewportAdapter.GetScaleMatrix()
            );

            Game.SpriteBatch.Draw(Assets.GetTexture2D("pixel"), new Rectangle(0, 0, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y), null, Color.White);

            Game.SpriteBatch.End();
        }
    }
}
