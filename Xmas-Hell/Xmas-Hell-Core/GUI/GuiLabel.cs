using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace XmasHell.GUI
{
    public class GuiLabel
    {
        public string Text;
        public Vector2 Position;
        public Color Color;

        public GuiLabel(string text, Vector2 position, Color color)
        {
            Text = text;
            Position = position;
            Color = color;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), Text, Position, Color);
        }
    }
}
