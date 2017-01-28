using System;
using Microsoft.Xna.Framework;
using SpriterDotNet;
using XmasHell.Geometry;

namespace XmasHell.Physics.Collision
{
    public class SpriterCollisionConvexPolygon : CollisionConvexPolygon
    {
        private ISpriterPhysicsEntity _spriterPhysicsEntity;
        private String _spritePartName;
        private SpriterFile _spriterPartFile;
        private int _spriterFileId;
        private Vector2 _relativePosition;

        public SpriterCollisionConvexPolygon(ISpriterPhysicsEntity entity, String spritePartName,
            Vector2? relativePosition = null, float scale = 1f) : base(entity)
        {
            _spriterPhysicsEntity = entity;
            _spritePartName = spritePartName;
            _spriterPartFile = FindSpriterFile(spritePartName);
            _relativePosition = relativePosition ?? Vector2.Zero;

            Vertices.Add(Vector2.Zero * scale);
            Vertices.Add(new Vector2(_spriterPartFile.Width, 0) * scale);
            Vertices.Add(new Vector2(_spriterPartFile.Width, _spriterPartFile.Height) * scale);
            Vertices.Add(new Vector2(0, _spriterPartFile.Height) * scale);
        }

        private SpriterFile FindSpriterFile(String fileName)
        {
            var currentAnimator = _spriterPhysicsEntity.GetCurrentAnimator();

            if (currentAnimator == null)
                return null;

            _spriterFileId = Array.FindIndex(currentAnimator.Entity.Spriter.Folders[0].Files,
                (file) => file.Name == fileName);

            return _spriterFileId == -1 ? null : currentAnimator.Entity.Spriter.Folders[0].Files[_spriterFileId];
        }

        public override Vector2 GetWorldPosition(Vector2 vertex)
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
                var offset = new Vector2(spriteData.X, -spriteData.Y);

                vertex.X *= spriteData.ScaleX;
                vertex.Y *= spriteData.ScaleY;

                // Compute origin point
                var pivotX = (_spriterPartFile.Width * spriteData.PivotX) + spriteData.X;
                var pivotY = (_spriterPartFile.Height * spriteData.PivotY) - spriteData.Y;

                var origin =
                    currentAnimator.Position +
                    new Vector2(pivotX, pivotY) -
                    new Vector2(_spriterPartFile.Width / 2f, _spriterPartFile.Height / 2f);

                var worldPosition =
                    currentAnimator.Position +
                    offset +
                    vertex +
                    _relativePosition -
                    new Vector2(_spriterPartFile.Width / 2f, _spriterPartFile.Height / 2f);

                var rotation = -spriteData.Angle;
                rotation = MathHelper.WrapAngle(MathHelper.ToRadians(rotation));

                // Take the animation angle into account
                worldPosition = MathHelperExtension.RotatePoint(
                    worldPosition, rotation, origin
                );

                // Take the global angle into account
                worldPosition = MathHelperExtension.RotatePoint(
                    worldPosition, currentAnimator.Rotation, currentAnimator.Position
                );


                return worldPosition;
            }

            return currentAnimator.Position + vertex;
        }
    }
}