using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

namespace Xmas_Hell.Sprites
{
    class StringSprite
    {
        public BitmapFont Font;
        public string Content;
        public Vector2 Position;
        public Color Color;

        public StringSprite(BitmapFont font, string content, Vector2 position, Color color)
        {
            Font = font;
            Content = content;
            Position = position;
            Color = color;
        }
    }
}