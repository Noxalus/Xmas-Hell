using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XmasHell.Entities.Bosses
{
    public abstract class AbstractBossBehaviour
    {
        protected Boss Boss;

        protected AbstractBossBehaviour(Boss boss)
        {
            Boss = boss;
        }

        public virtual void Start()
        {
        }

        public virtual void Stop()
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