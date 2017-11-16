using Microsoft.Xna.Framework;

namespace XmasHell.Physics
{
    public interface IPhysicsEntity
    {
        Vector2 Position();
        float Rotation();
        Vector2 ScaleVector(); // Avoid "Scale" name because it's already used by BulletML's Bullet class for a single float
        Vector2 Origin();
        void TakeDamage(float damage);
    }
}