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
        public SpriterSubstituteEntity SubstituteEntity;

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

        public SpriterGuiButton(
            ViewportAdapter viewportAdapter,
            String buttonName,
            String spritePartCompleteFilename,
            CustomSpriterAnimator animator,
            CustomSpriterAnimator referenceAnimator) :
            base(viewportAdapter, buttonName)
        {
            SubstituteEntity = new SpriterSubstituteEntity(Path.GetFileName(spritePartCompleteFilename), referenceAnimator, animator);
            referenceAnimator.AddHiddenTexture(Path.ChangeExtension(spritePartCompleteFilename, null));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            SubstituteEntity.Update(gameTime);
        }
    }
}
