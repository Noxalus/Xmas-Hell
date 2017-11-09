using Microsoft.Xna.Framework;

namespace XmasHell.Background
{
    public class StarBackground : AbstractBackground
    {
        public StarBackground(XmasHell game) : base(game)
        {
            BackgroundEffect = Assets.GetShader("Graphics/Shaders/StarBackground");
        }

        public override void Update(GameTime gameTime)
        {
            // Change speed/orientation
        }
    }
}