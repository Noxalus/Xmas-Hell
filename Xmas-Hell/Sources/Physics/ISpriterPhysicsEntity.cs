using Microsoft.Xna.Framework;
using SpriterDotNet.MonoGame;

namespace XmasHell.Physics
{
    public interface ISpriterPhysicsEntity : IPhysicsEntity
    {
        MonoGameAnimator GetCurrentAnimator();
    }
}