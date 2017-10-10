using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace XmasHell.GUI
{
    public class AbstractGuiLabel
    {
        public string Text;
        protected Vector2 Position;
        protected float Rotation;
        protected Vector2 Scale;
        protected Color Color;
        protected BitmapFont Font;
        protected bool Center;

        public AbstractGuiLabel(string text, BitmapFont font, Vector2 position, Color color, bool center = false)
        {
            Text = text;
            Position = position;
            Color = color;

            Font = font;
            Center = center;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Text, Position, Color, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }
    }
}
