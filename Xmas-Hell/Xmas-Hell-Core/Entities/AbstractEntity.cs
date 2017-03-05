using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace XmasHell.Entities
{
    abstract class AbstractEntity
    {
        private Vector2 _position;
        protected Vector2 Acceleration;
        protected float Speed;
        protected Vector2 TargetPosition;
        protected bool TargetingPosition;
        protected Vector2 TargetDirection;

        protected AbstractEntity()
        {
            // Default entity values
            Speed = 500f;
            Acceleration = Vector2.One;
        }

        public virtual Vector2 Position()
        {
            return _position;
        }

        public virtual void Position(Vector2 value)
        {
            _position = value;
        }

        public void MoveTo(Vector2 position, bool force = false)
        {
            if (TargetingPosition && !force)
                return;

            TargetingPosition = true;
            TargetPosition = position;
            TargetDirection = Vector2.Normalize(position - Position());
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!TargetDirection.Equals(Vector2.Zero))
            {
                var currentPosition = Position();
                var distance = Vector2.Distance(currentPosition, TargetPosition);
                var deltaDistance = Speed * gameTime.GetElapsedSeconds();

                if (distance < deltaDistance)
                {
                    TargetingPosition = false;
                    TargetDirection = Vector2.Zero;
                    Position(TargetPosition);
                }
                else
                {
                    // TODO: Perform some cubic interpolation
                    Position(currentPosition + (TargetDirection * deltaDistance) * Acceleration);
                }
            }
            else
            {
                //var newPosition = Vector2.Zero;
                //var lerpAmount = (float)(_targetPositionTime.TotalSeconds / _targetPositionTimer.TotalSeconds);

                //newPosition.X = MathHelper.SmoothStep(_targetPosition.X, _initialPosition.X, lerpAmount);
                //newPosition.Y = MathHelper.SmoothStep(_targetPosition.Y, _initialPosition.Y, lerpAmount);

                //if (lerpAmount < 0.001f)
                //{
                //    TargetingPosition = false;
                //    _targetPositionTimer = TimeSpan.Zero;
                //    CurrentAnimator.Position = _targetPosition;
                //}
                //else
                //    _targetPositionTime -= gameTime.ElapsedGameTime;

                //CurrentAnimator.Position = newPosition;
            }
        }
    }
}
