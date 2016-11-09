using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XmasHell.Physics.Collision
{
    public abstract class CollisionElement
    {
        // The linked entity
        public IPhysicsEntity Entity;

        protected CollisionElement(IPhysicsEntity entity)
        {
            Entity = entity;
        }

        protected Matrix GetMatrix()
        {
            var scale = Entity.Scale();
            var rotation = Entity.Rotation();
            var pivot = Entity.Pivot();
            var position = Entity.Position();

            pivot = Vector2.Zero;

            return
                Matrix.CreateTranslation(-pivot.X, -pivot.Y, 0.0f) *
                Matrix.CreateScale(Math.Abs(scale.X), Math.Abs(scale.Y), 1.0f) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(position.X + pivot.X, position.Y + pivot.Y, 0.0f);
        }

        public abstract bool Intersects(CollisionCircle element);
        public abstract bool Intersects(CollisionConvexPolygon element);

        public bool Intersects(CollisionElement element)
        {
            var circle = element as CollisionCircle;
            if (circle != null)
                return Intersects(circle);

            var convexPolygon = element as CollisionConvexPolygon;
            if (convexPolygon != null)
                return Intersects(convexPolygon);

            throw new System.NotImplementedException();
        }

        // For debug purpose
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}