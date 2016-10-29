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

        public abstract bool Intersects(CollisionElement element);

        // For debug purpose
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}