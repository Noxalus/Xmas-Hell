using Microsoft.Xna.Framework;

namespace XmasHell.Physics
{
    public interface ICollidable
    {
        bool Intersects(ICollidable collidable);
    }
}