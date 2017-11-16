using Microsoft.Xna.Framework;
using SpriterDotNet.MonoGame;

namespace XmasHell.Physics
{
    class SpriterPhysicsEntity : IPhysicsEntity
    {
        private readonly MonoGameAnimator _animator;

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

        public virtual Vector2 Origin()
        {
            return Vector2.Zero;
        }

        public virtual Vector2 ScaleVector()
        {
            if (_animator.FrameData != null && _animator.FrameData.SpriteData.Count > 0)
            {
                var spriteData = _animator.FrameData.SpriteData[0];
                return new Vector2(spriteData.ScaleX, spriteData.ScaleY);
            }

            return _animator.Scale;
        }

        public void TakeDamage(float damage)
        {
            // Nothing
        }

        public SpriterPhysicsEntity(MonoGameAnimator animator)
        {
            _animator = animator;
        }
    }
}