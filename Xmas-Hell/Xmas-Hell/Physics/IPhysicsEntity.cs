using Microsoft.Xna.Framework;

namespace XmasHell.Physics
{
    public interface IPhysicsEntity
    {
        Vector2 Position();
        Vector2 LocalPosition();
        float Rotation();
        Vector2 Pivot();
        Vector2 Scale();
    }
}