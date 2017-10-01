using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;
using SpriterDotNet.MonoGame;
using System;
using XmasHell.Spriter;

namespace XmasHell.GUI
{
    public class SpriterGuiButton : AbstractGuiButton
    {
        public MonoGameAnimator Animator;
        private String _spritePartFilename;

        public override Vector2 Position()
        {
            return Animator.Position;
        }

        public override void Scale(Vector2 value)
        {
            Animator.Scale = value;
        }

        public override float Rotation()
        {
            return Animator.Rotation;
        }

        public override void Rotation(float value)
        {
            Animator.Rotation = value;
        }

        public override void Position(Vector2 value)
        {
            Animator.Position = value;
        }

        public override Vector2 Scale()
        {
            return Animator.Scale;
        }

        public override BoundingRectangle BoundingRectangle()
        {
            var spriteSize = SpriterUtils.GetSpriterFileSize(_spritePartFilename, Animator);
            return new BoundingRectangle(Position(), new Size2(spriteSize.X / 2, spriteSize.Y / 2));
        }

        public SpriterGuiButton(ViewportAdapter viewportAdapter, String buttonName, String spritePartFilename, MonoGameAnimator animator) : base(viewportAdapter, buttonName)
        {
            Animator = animator;
            _spritePartFilename = spritePartFilename;
        }
    }
}
