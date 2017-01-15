using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SpriterDotNet.MonoGame;

namespace XmasHell.Physics
{
    class SpriterPhysicsEntity : IPhysicsEntity
    {
        public virtual Vector2 Position()
        {
            var currentPosition = _animator.Position;
            if (_animator.FrameData != null && _animator.FrameData.SpriteData.Count > 0)
            {
                var spriteData = _animator.FrameData.SpriteData[0];
                return currentPosition + new Vector2(spriteData.X, -spriteData.Y);
            }

            return currentPosition;
        }

        public virtual float Rotation()
        {
            if (_animator.FrameData != null && _animator.FrameData.SpriteData.Count > 0)
            {
                var spriteData = _animator.FrameData.SpriteData[0];
                return MathHelper.ToRadians(-spriteData.Angle);
            }

            return _animator.Rotation;
        }

        public virtual Vector2 Scale()
        {
            if (_animator.FrameData != null && _animator.FrameData.SpriteData.Count > 0)
            {
                var spriteData = _animator.FrameData.SpriteData[0];
                return new Vector2(spriteData.ScaleX, spriteData.ScaleY);
            }

            return _animator.Scale;
        }

        private readonly MonoGameAnimator _animator;

        public SpriterPhysicsEntity(MonoGameAnimator animator)
        {
            _animator = animator;
        }
    }
}