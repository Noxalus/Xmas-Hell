using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
