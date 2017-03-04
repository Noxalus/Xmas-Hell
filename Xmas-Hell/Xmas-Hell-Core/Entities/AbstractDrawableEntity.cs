using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace XmasHell.Entities
{
    class AbstractDrawableEntity : AbstractEntity
    {
        protected Sprite Sprite;

        public override Vector2 GetPosition()
        {
            return Sprite.Position;
        }

        public override void SetPosition(Vector2 value)
        {
            Sprite.Position = value;
        }

        protected AbstractDrawableEntity(Sprite sprite) : base()
        {
            Sprite = sprite;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite);
        }
    }
}
