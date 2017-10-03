using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using SpriterDotNet.MonoGame;
using System;
using System.IO;
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
            String spritePartCompleteFilename,
            CustomSpriterAnimator animator,
            CustomSpriterAnimator referenceAnimator) :
            base(viewportAdapter, buttonName)
        {
            Animator = animator;
            _referenceAnimator = referenceAnimator;
            _spritePartFilename = Path.GetFileName(spritePartCompleteFilename);

            referenceAnimator.AddHiddenTexture(Path.ChangeExtension(spritePartCompleteFilename, null));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Synchronize();
        }

        private void Synchronize()
        {
            // Synchronize current GUI button animator with the related dummy element from the Spriter file
            var spriterDummyData = SpriterUtils.GetSpriterFileData(_spritePartFilename, _referenceAnimator);
            var dummyPosition = new Vector2(spriterDummyData.X, -spriterDummyData.Y);
            var dummyScale = new Vector2(spriterDummyData.ScaleX, spriterDummyData.ScaleY);

            Position(_referenceAnimator.Position + dummyPosition);
            Rotation(spriterDummyData.Angle);
            Scale(dummyScale);
            Animator.Color = new Color(Animator.Color, spriterDummyData.Alpha);
        }
    }
}
