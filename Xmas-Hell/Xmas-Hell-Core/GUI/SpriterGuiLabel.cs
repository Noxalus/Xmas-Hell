using Microsoft.Xna.Framework;
using XmasHell.Spriter;

namespace XmasHell.GUI
{
    public class SpriterGuiLabel : AbstractGuiLabel
    {
        private string _replacedPartFilename;
        private CustomSpriterAnimator _referenceAnimator;

        public SpriterGuiLabel(
            string text,
            string spritePartCompleteFilename,
            CustomSpriterAnimator referenceAnimator) :
            base(text, Vector2.Zero, Color.Black)
        {
            _replacedPartFilename = spritePartCompleteFilename;
            _referenceAnimator = referenceAnimator;
            _referenceAnimator.AddHiddenTexture(_replacedPartFilename);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Synchronize();
        }

        public void Synchronize()
        {
            // Synchronize current GUI button animator with the related dummy element from the Spriter file
            var spriterDummyData = SpriterUtils.GetSpriterFileData(_replacedPartFilename, _referenceAnimator);

            if (spriterDummyData != null)
            {
                var dummyPosition = new Vector2(spriterDummyData.X, -spriterDummyData.Y);
                var dummyScale = new Vector2(spriterDummyData.ScaleX, spriterDummyData.ScaleY);

                Position = _referenceAnimator.Position + dummyPosition;
                Rotation = -MathHelper.ToRadians(spriterDummyData.Angle);
                Scale = dummyScale;
                Position.X -= (Font.MeasureString(Text).Width / 2f) * Scale.X;
                Position.Y -= (Font.MeasureString(Text).Height / 2f) * Scale.Y;
                Color = new Color(Color, spriterDummyData.Alpha);
            }
        }
    }
}
