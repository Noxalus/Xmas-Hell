using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
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

            Vertices.Add(Vector2.Zero);
            Vertices.Add(new Vector2(_spriterPartFile.Width, 0) * scale);
            Vertices.Add(new Vector2(_spriterPartFile.Width, _spriterPartFile.Height) * scale);
            Vertices.Add(new Vector2(0, _spriterPartFile.Height) * scale);
        }

        private SpriterFile FindSpriterFile(string fileName)
        {
            var currentAnimator = _spriterPhysicsEntity.GetCurrentAnimator();

            if (currentAnimator == null)
                throw new ArgumentException("Please make sure that a Spriter animator is linked to the given entity.");

            _spriterFileId = Array.FindIndex(currentAnimator.Entity.Spriter.Folders[0].Files,
                (file) => file.Name == fileName);

            if (_spriterFileId == -1)
                throw new ArgumentException("Please make sure that the given Spriter entity has a sprite part named: " + _spritePartName);

            return currentAnimator.Entity.Spriter.Folders[0].Files[_spriterFileId];
        }

        public override Vector2 GetWorldPosition(Vector2 vertex)
        {
            var currentAnimator = _spriterPhysicsEntity.GetCurrentAnimator();
            var worldPosition = currentAnimator.Position + vertex;

            if (currentAnimator.FrameData != null)
            {
                // Compute translation
                var spriteData = currentAnimator.FrameData.SpriteData.Find((so) => so.FileId == _spriterFileId);
                var animationOffset = new Vector2(spriteData.X, -spriteData.Y);

                vertex.X *= spriteData.ScaleX;
                vertex.Y *= spriteData.ScaleY;

                var spriteCenter = new Vector2(
                    _spriterPartFile.Width * (1 - spriteData.PivotX) * spriteData.ScaleX + spriteData.X,
                    _spriterPartFile.Height * (1 - spriteData.PivotY) * spriteData.ScaleY - spriteData.Y
                );
                var worldTopLeftCornerPosition = currentAnimator.Position - spriteCenter;

                worldPosition = worldTopLeftCornerPosition + vertex + animationOffset + _relativePosition;

                // Compute rotation
                var origin = currentAnimator.Position;

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
            }

            return worldPosition;
        }
    }
}