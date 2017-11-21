using System;
using Microsoft.Xna.Framework;
using SpriterDotNet;
using XmasHell.Spriter;
using XmasHell.Extensions;

namespace XmasHell.Physics.Collision
{
    public class SpriterCollisionConvexPolygon : CollisionConvexPolygon
    {
        private ISpriterPhysicsEntity _spriterPhysicsEntity;
        private SpriterFile _spriterPartFile;
        private Vector2 _relativePosition;

        public SpriterCollisionConvexPolygon(ISpriterPhysicsEntity entity, String spritePartName,
            Vector2? relativePosition = null, Vector2? scaleVector = null) : base(entity)
        {
            var scale = Vector2.One;

            if (scaleVector.HasValue)
                scale = scaleVector.Value;

            _spriterPhysicsEntity = entity;
            _spriterPartFile = SpriterUtils.GetSpriterFile(spritePartName, _spriterPhysicsEntity.GetCurrentAnimator());
            _relativePosition = relativePosition ?? Vector2.Zero;

            Vertices.Add(Vector2.Zero);
            Vertices.Add(new Vector2(_spriterPartFile.Width, 0) * scale);
            Vertices.Add(new Vector2(_spriterPartFile.Width, _spriterPartFile.Height) * scale);
            Vertices.Add(new Vector2(0, _spriterPartFile.Height) * scale);
        }

        public override Vector2 GetWorldPosition(Vector2 vertex)
        {
            var currentAnimator = _spriterPhysicsEntity.GetCurrentAnimator();
            var worldPosition = currentAnimator.Position + vertex;

            if (currentAnimator.FrameData != null)
            {
                // Compute translation
                var spriteData = currentAnimator.FrameData.SpriteData.Find((so) => so.FileId == _spriterPartFile.Id);

                if (spriteData == null)
                    return worldPosition;

                var animationOffset = new Vector2(spriteData.X, -spriteData.Y);
                var scale = new Vector2(spriteData.ScaleX, spriteData.ScaleY) * currentAnimator.Scale;
                var realPivotPosition = new Vector2(1 - spriteData.PivotX, 1 - spriteData.PivotY);

                vertex *= scale;

                var spriteCenter = new Vector2(
                    _spriterPartFile.Width * realPivotPosition.X,
                    _spriterPartFile.Height * realPivotPosition.Y
                );
                var worldTopLeftCornerPosition = currentAnimator.Position - (spriteCenter * scale);

                worldPosition = worldTopLeftCornerPosition + vertex + animationOffset + _relativePosition;

                // Compute rotation
                var pivot = new Vector2(
                    (_spriterPartFile.Width * realPivotPosition.X) + spriteData.X,
                    (_spriterPartFile.Height * realPivotPosition.Y) - spriteData.Y
                );

                var origin = currentAnimator.Position + pivot - spriteCenter;

                var rotation = -spriteData.Angle;
                rotation = MathHelper.WrapAngle(MathHelper.ToRadians(rotation));

                // Take the animation angle into account
                worldPosition = MathExtension.RotatePoint(
                    worldPosition, rotation, origin
                );

                // Take the global angle into account
                worldPosition = MathExtension.RotatePoint(
                    worldPosition, currentAnimator.Rotation, currentAnimator.Position
                );
            }

            return worldPosition;
        }
    }
}