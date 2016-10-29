using Microsoft.Xna.Framework;

namespace Xmas_Hell.Physics
{
    public interface IPhysicsEntity
    {
        Vector2 Position();
        float Rotation();
        Vector2 Scale();
    }
}