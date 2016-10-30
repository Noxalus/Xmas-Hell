using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace Xmas_Hell.Physics.Collision
{
    public class CollisionCircle : CollisionElement
    {
        public float Radius;

        private Vector2 _relativePosition;

        public CollisionCircle(IPhysicsEntity entity, Vector2 relativePosition, float radius) : base(entity)
        {
            Entity = entity;
            _relativePosition = relativePosition;
            Radius = radius;
        }

        private Vector2 GetCenter()
        {
            // Wrong: Use all data from the entity Transform (rotation + scale too)
            return Entity.Position() + _relativePosition;
        }

        private bool Intersects(CollisionCircle circle)
        {
            float dx = circle.GetCenter().X - GetCenter().X;
            float dy = circle.GetCenter().Y - GetCenter().Y;
            float radii = Radius + circle.Radius;

            return (dx * dx) + (dy * dy) < radii * radii;
        }

        private bool Intersects(CollisionConvexPolygon convexPolygon)
        {
            return false;
        }

        public override bool Intersects(CollisionElement element)
        {
            var circle = element as CollisionCircle;
            if (circle != null)
                return Intersects(circle);

            var convexPolygon = element as CollisionConvexPolygon;
            if (convexPolygon != null)
                return Intersects(convexPolygon);

            throw new System.NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(GetCenter().X, GetCenter().Y, Radius, 10, Color.White);
        }
    }
}