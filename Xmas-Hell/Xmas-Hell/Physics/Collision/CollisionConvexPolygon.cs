using Microsoft.Xna.Framework.Graphics;

namespace Xmas_Hell.Physics.Collision
{
    class CollisionConvexPolygon : CollisionElement
    {
        public override bool Intersects(CollisionElement element)
        {
            throw new System.NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            throw new System.NotImplementedException();
        }

        public CollisionConvexPolygon(IPhysicsEntity entity) : base(entity)
        {
        }
    }
}