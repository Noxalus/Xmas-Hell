using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace XmasHell.GUI
{
    public class SpriteGuiButton : AbstractGuiButton
    {
        public Sprite Sprite;

        public override Vector2 Position()
        {
            return Sprite.Position;
        }

        public override void Scale(Vector2 value)
        {
            Sprite.Scale = value;
        }

        public override float Rotation()
        {
            return Sprite.Rotation;
        }

        public override void Rotation(float value)
        {
            Sprite.Rotation = value;
        }

        public override void Position(Vector2 value)
        {
            Sprite.Position = value;
        }

        public override Vector2 Scale()
        {
            return Sprite.Scale;
        }

        public override BoundingRectangle BoundingRectangle()
        {
            return Sprite.BoundingRectangle;
        }

        public SpriteGuiButton(ViewportAdapter viewportAdapter, String name, Sprite sprite) : base(viewportAdapter, name)
        {
            Sprite = sprite;
        }
    }
}
