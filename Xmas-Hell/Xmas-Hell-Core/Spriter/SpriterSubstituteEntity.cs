using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Diagnostics;

namespace XmasHell.Spriter
{
    public class SpriterSubstituteEntity
    {
        public CustomSpriterAnimator SubstituteAnimator;
        private string _replacedPartFilename;
        private CustomSpriterAnimator _referenceAnimator;
        private SpriterDotNet.SpriterFile _replacedSpriterFile;
        private Size2 _replacedSpriterFileHalfSize;
        private bool _synchronize = true;
        private bool _firstFrame = true;

        public BoundingRectangle BoundingRectangle()
        {
            return new BoundingRectangle(SubstituteAnimator.Position, _replacedSpriterFileHalfSize);
        }

        public void EnableSynchronization(bool value)
        {
            _synchronize = value;
        }

        public SpriterSubstituteEntity(string replacedPartFilename, CustomSpriterAnimator referenceAnimator, CustomSpriterAnimator substituteAnimator)
        {
            _replacedPartFilename = replacedPartFilename;
            _referenceAnimator = referenceAnimator;
            SubstituteAnimator = substituteAnimator;

            _replacedSpriterFile = SpriterUtils.GetSpriterFileStaticData(_replacedPartFilename, SubstituteAnimator);
            _replacedSpriterFileHalfSize = new Size2(_replacedSpriterFile.Width / 2, _replacedSpriterFile.Height / 2);

            referenceAnimator.AddHiddenTexture(replacedPartFilename);
        }

        public void Reset()
        {
            _firstFrame = true;
        }

        public void Update(GameTime gameTime)
        {
            if (_synchronize)
                Synchronize();

            _firstFrame = false;
        }

        public void Synchronize()
        {
            // Synchronize current GUI button animator with the related dummy element from the Spriter file
            var spriterDummyData = SpriterUtils.GetSpriterFileData(_replacedPartFilename, _referenceAnimator);

            if (spriterDummyData != null && !_firstFrame)
            {
                var dummyPosition = new Vector2(spriterDummyData.X, -spriterDummyData.Y);
                var dummyScale = new Vector2(spriterDummyData.ScaleX, spriterDummyData.ScaleY);

                SubstituteAnimator.Position = _referenceAnimator.Position + dummyPosition;
                SubstituteAnimator.Rotation = -MathHelper.ToRadians(spriterDummyData.Angle);
                SubstituteAnimator.Scale = dummyScale;
                SubstituteAnimator.Color = new Color(SubstituteAnimator.Color, spriterDummyData.Alpha);
            }
            else if (_firstFrame)
            {
                // Workaround to avoid desync on the first frame
                SubstituteAnimator.Scale = Vector2.Zero;
            }

        }
    }
}
