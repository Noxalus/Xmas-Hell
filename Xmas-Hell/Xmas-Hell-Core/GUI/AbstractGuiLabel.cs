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
        public Vector2 Scale;
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
            Scale = Vector2.One;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var position = Position;

            if (Center)
            {
                position.X -= (Font.MeasureString(Text).Width / 2f) * Scale.X;
                position.Y -= (Font.MeasureString(Text).Height / 2f) * Scale.Y;
            }
            else
            {
                //position.X -= 1 * Scale.X;
                //position.Y -= (Font.MeasureString(Text).Height / 2f) * Scale.Y;
            }

            spriteBatch.DrawString(Font, Text, position, Color, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }
    }
}
