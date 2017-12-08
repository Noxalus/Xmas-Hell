using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.IO;
using XmasHell.Spriter;

namespace XmasHell.GUI
{
    public class SpriterGuiButton : AbstractGuiButton
    {
        public SpriterSubstituteEntity SubstituteEntity;
        private string _animationName;
        private string _clickAnimationName;

        public CustomSpriterAnimator Animator()
        {
            return SubstituteEntity.SubstituteAnimator;
        }

        public override Vector2 Position()
        {
            return SubstituteEntity.SubstituteAnimator.Position;
        }

        public override void Scale(Vector2 value)
        {
            SubstituteEntity.SubstituteAnimator.Scale = value;
        }

        public override float Rotation()
        {
            return SubstituteEntity.SubstituteAnimator.Rotation;
        }

        public override void Rotation(float value)
        {
            SubstituteEntity.SubstituteAnimator.Rotation = value;
        }

        public override void Position(Vector2 value)
        {
            SubstituteEntity.SubstituteAnimator.Position = value;
        }

        public override Vector2 Scale()
        {
            return SubstituteEntity.SubstituteAnimator.Scale;
        }

        public override BoundingRectangle BoundingRectangle()
        {
            return SubstituteEntity.BoundingRectangle();
        }

        public void Enable(bool value)
        {
            Enabled = value;
        }

        public void Synchronize()
        {
            SubstituteEntity.Synchronize();
        }

        public SpriterGuiButton(
            ViewportAdapter viewportAdapter,
            String buttonName,
            String spritePartCompleteFilename,
            CustomSpriterAnimator animator,
            CustomSpriterAnimator referenceAnimator,
            string animationName = null,
            string clickAnimationName = null) :
            base(viewportAdapter, buttonName)
        {
            SubstituteEntity = new SpriterSubstituteEntity(Path.GetFileName(spritePartCompleteFilename), referenceAnimator, animator);
            _animationName = animationName;
            _clickAnimationName = clickAnimationName;

            if (_animationName != null)
                SubstituteEntity.SubstituteAnimator.Play(_animationName);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputDownStateChanged)
            {
                if (InputDown && _clickAnimationName != null)
                    SubstituteEntity.SubstituteAnimator.Play(_clickAnimationName);
                else if (_animationName != null)
                    SubstituteEntity.SubstituteAnimator.Play(_animationName);
            }

            SubstituteEntity.Update(gameTime);
        }
    }
}
