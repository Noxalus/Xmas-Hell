using Microsoft.Xna.Framework;

namespace Xmas_Hell.Entities.Bosses
{
    public abstract class AbstractBossBehaviour
    {
        protected Boss Boss;

        protected AbstractBossBehaviour(Boss boss)
        {
            Boss = boss;
        }

        public virtual void Reset()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}