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
        public MonoGameAnimator _referenceAnimator;
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

        public SpriterGuiButton(
            ViewportAdapter viewportAdapter,
            String buttonName,
            String spritePartFilename,
            MonoGameAnimator animator,
            MonoGameAnimator referenceAnimator) :
            base(viewportAdapter, buttonName)
        {
            Animator = animator;
            _referenceAnimator = referenceAnimator;
            _spritePartFilename = spritePartFilename;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Place buttons according to their dummy positions on the Spriter file
            var spriterPlayButtonDummyPosition = SpriterUtils.GetSpriterFilePosition(_spritePartFilename, _referenceAnimator);
            Position(_referenceAnimator.Position + spriterPlayButtonDummyPosition);
        }
    }
}
