using Microsoft.Xna.Framework;

namespace Xmas_Hell.Sprites
{
    public abstract class AbstractSprite
    {
        private int _drawOrder;

        public int DrawOrder()
        {
            return _drawOrder;
        }

        public void DrawOrder(int value)
        {
            _drawOrder = value;
        }

        public abstract void Draw(GameTime gameTime);
    }
}