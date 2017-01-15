using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using SpriterDotNet;
using SpriterDotNet.MonoGame;

namespace XmasHell.Physics.Collision
{
    public class SpriterCollisionCircle : CollisionCircle
    {
        private ISpriterPhysicsEntity _spriterPhysicsEntity;
        private String _spritePartName;
        private SpriterFile _spriterPartFile;
        private Vector2 _relativePosition;
        private float _scale;

        public SpriterCollisionCircle(ISpriterPhysicsEntity entity, String spritePartName, Vector2? relativePosition = null, float scale = 1f) : base(entity)
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

            var foundIndex = Array.FindIndex(currentAnimator.Entity.Spriter.Folders[0].Files, (file) => file.Name == fileName);

            if (foundIndex == -1)
                return null;

            return currentAnimator.Entity.Spriter.Folders[0].Files[foundIndex];
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
                var spriteData = currentAnimator.FrameData.SpriteData[0];
                var widthRadius = _spriterPartFile.Width * spriteData.ScaleX;
                var heightRadius = _spriterPartFile.Height * spriteData.ScaleY;

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
                var spriteData = currentAnimator.FrameData.SpriteData[0];
                var pivotX = (_spriterPartFile.Width * spriteData.PivotX) + spriteData.X;
                var pivotY = (_spriterPartFile.Height * spriteData.PivotY) - spriteData.Y;

                return
                    currentAnimator.Position +
                    new Vector2(pivotX, pivotY) -
                    new Vector2(_spriterPartFile.Width / 2f, _spriterPartFile.Height / 2f) +
                    _relativePosition;
            }

            return currentAnimator.Position;
        }
    }
}