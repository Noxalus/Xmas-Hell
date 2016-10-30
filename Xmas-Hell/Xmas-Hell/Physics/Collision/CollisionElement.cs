using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xmas_Hell.Physics.Collision
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
            var position = Entity.Position();

            return
                Matrix.CreateScale(Math.Abs(scale.X), Math.Abs(scale.Y), 1.0f) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(position.X, position.Y, 0.0f);
        }

        public abstract bool Intersects(CollisionElement element);

        // For debug purpose
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}