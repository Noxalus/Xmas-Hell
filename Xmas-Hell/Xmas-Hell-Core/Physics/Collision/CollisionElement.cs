using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XmasHell.Physics.Collision
{
    public abstract class CollisionElement : ICollidable
    {
        // The linked entity
        public IPhysicsEntity Entity;
        protected CollisionElement(IPhysicsEntity entity)
        {
            Entity = entity;
        }

        protected Matrix GetMatrix()
        {
            var scale = Entity.ScaleVector();
            var rotation = Entity.Rotation();
            var position = Entity.Position();
            //var origin = Entity.Origin();

            return
                Matrix.CreateScale(Math.Abs(scale.X), Math.Abs(scale.Y), 1.0f) *
                //Matrix.CreateTranslation(-origin.X, -origin.Y, 0f) *
                Matrix.CreateRotationZ(rotation) *
                //Matrix.CreateTranslation(origin.X, origin.Y, 0f) *
                Matrix.CreateTranslation(position.X, position.Y, 0.0f)
            ;
        }

        public abstract bool Intersects(CollisionCircle element);
        public abstract bool Intersects(CollisionConvexPolygon element);

        public bool Intersects(ICollidable element)
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