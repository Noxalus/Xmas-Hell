using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace XmasHell.Physics.Collision
{
    public class CollisionCircle : CollisionElement
    {
        private float _initialRadius;
        private Vector2 _relativePosition;

        public CollisionCircle(IPhysicsEntity entity, Vector2 relativePosition, float radius) : base(entity)
        {
            _relativePosition = relativePosition;
            _initialRadius = radius;
        }

        protected CollisionCircle(IPhysicsEntity entity) : base(entity)
        {
        }

        public virtual float GetRadius()
        {
            return _initialRadius * Entity.Scale().X;
        }

        public virtual Vector2 GetCenter()
        {
            var entityTransformMatrix = GetMatrix();
            var localCenter = _relativePosition;
            var vertexPosition = new Vector3(localCenter.X, localCenter.Y, 0f);
            var transformedVertexPosition = Vector3.Transform(vertexPosition, entityTransformMatrix);
            var worldCenter = new Vector2(transformedVertexPosition.X, transformedVertexPosition.Y);

            return worldCenter;
        }

        public override bool Intersects(CollisionCircle circle)
        {
            float dx = circle.GetCenter().X - GetCenter().X;
            float dy = circle.GetCenter().Y - GetCenter().Y;
            float radii = GetRadius() + circle.GetRadius();

            return (dx * dx) + (dy * dy) < radii * radii;
        }

        public override bool Intersects(CollisionConvexPolygon convexPolygon)
        {
            return convexPolygon.Intersects(this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(GetCenter().X, GetCenter().Y, GetRadius(), 10, Color.Red, 5f);
        }
    }
}