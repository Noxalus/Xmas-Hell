using System;
using Microsoft.Xna.Framework;
using SpriterDotNet;
using XmasHell.Extensions;

namespace XmasHell.Physics.Collision
{
    public class SpriterCollisionCircle : CollisionCircle
    {
        private ISpriterPhysicsEntity _spriterPhysicsEntity;
        private string _spritePartName;
        private SpriterFile _spriterPartFile;
        private int _spriterFileId;
        private Vector2 _relativePosition;
        private float _scale;

        public SpriterCollisionCircle(ISpriterPhysicsEntity entity, string spritePartName, Vector2? relativePosition = null, float scale = 1f) : base(entity)
        {
            _spriterPhysicsEntity = entity;
            _spritePartName = spritePartName;
            _spriterPartFile = FindSpriterFile(spritePartName);
            _relativePosition = relativePosition ?? Vector2.Zero;
            _scale = scale;
        }

        private SpriterFile FindSpriterFile(String fileName)
        {
            var currentAnimator = _spriterPhysicsEntity.GetCurrentAnimator();

            if (currentAnimator == null)
                return null;

            _spriterFileId = Array.FindIndex(currentAnimator.Entity.Spriter.Folders[0].Files, (file) => file.Name == fileName);

            if (_spriterFileId == -1)
                return null;

            return currentAnimator.Entity.Spriter.Folders[0].Files[_spriterFileId];
        }

        public override float GetRadius()
        {
            if (_spriterPartFile == null)
            {
                _spriterPartFile = FindSpriterFile(_spritePartName);

                if (_spriterPartFile == null)
                    return 0f;
            }

            var currentAnimator = _spriterPhysicsEntity.GetCurrentAnimator();

            if (currentAnimator.FrameData != null)
            {
                var spriteData = currentAnimator.FrameData.SpriteData.Find((so) => so.FileId == _spriterFileId);

                if (spriteData == null)
                    return 0f;

                var widthRadius = _spriterPartFile.Width * spriteData.ScaleX * currentAnimator.Scale.X;
                var heightRadius = _spriterPartFile.Height * spriteData.ScaleY * currentAnimator.Scale.Y;

                return ((widthRadius + heightRadius) / 4f) * _scale;
            }

            return 0f;
        }

        public override Vector2 GetCenter()
        {
            var currentAnimator = _spriterPhysicsEntity.GetCurrentAnimator();

            if (_spriterPartFile == null)
            {
                _spriterPartFile = FindSpriterFile(_spritePartName);

                if (_spriterPartFile == null)
                    return currentAnimator.Position;
            }

            if (currentAnimator.FrameData != null)
            {
                var spriteData = currentAnimator.FrameData.SpriteData.Find((so) => so.FileId == _spriterFileId);

                if (spriteData == null)
                    return currentAnimator.Position;

                var pivotX = (_spriterPartFile.Width * spriteData.PivotX) + spriteData.X;
                var pivotY = (_spriterPartFile.Height * spriteData.PivotY) - spriteData.Y;

                // Compute rotation
                var realPivotPosition = new Vector2(1 - spriteData.PivotX, 1 - spriteData.PivotY);
                var pivot = new Vector2(
                    (_spriterPartFile.Width * realPivotPosition.X) + spriteData.X,
                    (_spriterPartFile.Height * realPivotPosition.Y) - spriteData.Y
                );

                var spriteCenter = new Vector2(
                    _spriterPartFile.Width * realPivotPosition.X,
                    _spriterPartFile.Height * realPivotPosition.Y
                );

                var origin = currentAnimator.Position + pivot - spriteCenter;
                var rotation = -spriteData.Angle;
                rotation = MathHelper.WrapAngle(MathHelper.ToRadians(rotation));

                var centerPosition =
                    currentAnimator.Position +
                    new Vector2(pivotX, pivotY) -
                    new Vector2(_spriterPartFile.Width / 2f, _spriterPartFile.Height / 2f) +
                    _relativePosition * currentAnimator.Scale
                ;

                centerPosition = MathExtension.RotatePoint(
                    centerPosition, rotation, origin
                );

                centerPosition = MathExtension.RotatePoint(
                    centerPosition, currentAnimator.Rotation, currentAnimator.Position
                );

                return centerPosition;
            }

            return currentAnimator.Position;
        }
    }
}