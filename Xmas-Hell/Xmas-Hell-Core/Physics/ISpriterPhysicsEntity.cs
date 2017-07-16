using XmasHell.Spriter;

namespace XmasHell.Physics
{
    public interface ISpriterPhysicsEntity : IPhysicsEntity
    {
        CustomSpriterAnimator GetCurrentAnimator();
    }
}