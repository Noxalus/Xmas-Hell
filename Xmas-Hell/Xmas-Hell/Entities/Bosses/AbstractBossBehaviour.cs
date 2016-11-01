using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}